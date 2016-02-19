using Bam.Core;
namespace tiff
{
    [ModuleGroup("Thirdparty/tiff")]
    class CopyStandardHeaders :
        Publisher.Collation
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            // the build mode depends on whether this path has been set or not
            if (this.GeneratedPaths.ContainsKey(Key))
            {
                this.GeneratedPaths[Key].Aliased(this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders"));
            }
            else
            {
                this.RegisterGeneratedFile(Key, this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders"));
            }

            var tiffHeader = this.IncludeFile(this.CreateTokenizedString("$(packagedir)/libtiff/tiff.h"), ".");
            this.IncludeFile(this.CreateTokenizedString("$(packagedir)/libtiff/tiffvers.h"), ".", tiffHeader);
            this.IncludeFile(this.CreateTokenizedString("$(packagedir)/libtiff/tiffio.h"), ".", tiffHeader);
        }
    }

    [ModuleGroup("Thirdparty/tiff")]
    class GenerateConfHeader :
        C.ProceduralHeaderFile
    {
        protected override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders/tiffconf.h");
            }
        }

        protected override string Contents
        {
            get
            {
                if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
                {
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.CreateTokenizedString("$(packagedir)/libtiff/tiffconf.vc.h").Parse()))
                    {
                        return readFile.ReadToEnd();
                    }
                }
                else
                {
                    var contents = new System.Text.StringBuilder();
                    return contents.ToString();
                }
            }
        }
    }

    [ModuleGroup("Thirdparty/tiff")]
    class GenerateConfigHeader :
        C.ProceduralHeaderFile
    {
        protected override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders/tif_config.h");
            }
        }

        protected override string Contents
        {
            get
            {
                if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
                {
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.CreateTokenizedString("$(packagedir)/libtiff/tiffconf.vc.h").Parse()))
                    {
                        return readFile.ReadToEnd();
                    }
                }
                else
                {
                    var contents = new System.Text.StringBuilder();
                    return contents.ToString();
                }
            }
        }
    }

    [ModuleGroup("Thirdparty/tiff")]
    sealed class LibTiff :
        C.StaticLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            var source = this.CreateCSourceContainer("$(packagedir)/libtiff/*.c", filter: new System.Text.RegularExpressions.Regex(@"^((?!.*acorn)(?!.*apple)(?!.*atari)(?!.*msdos)(?!.*unix)(?!.*win3).*)$"));
            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                source.AddFiles("$(packagedir)/libtiff/tif_win32.c");
                if (this.Librarian is VisualCCommon.Librarian)
                {
                    this.CompileAgainst<WindowsSDK.WindowsSDK>(source);
                }
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                source.AddFiles("$(packagedir)/libtiff/tif_unix.c");
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                source.AddFiles("$(packagedir)/libtiff/tif_apple.c");
            }

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Graph.Instance.FindReferencedModule<CopyStandardHeaders>();
            var generateConf = Graph.Instance.FindReferencedModule<GenerateConfHeader>();
            var generateConfig = Graph.Instance.FindReferencedModule<GenerateConfigHeader>();
            source.DependsOn(copyStandardHeaders, generateConf, generateConfig);

            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (null != compiler)
                    {
                        compiler.IncludePaths.AddUnique(copyStandardHeaders.GeneratedPaths[Publisher.Collation.Key]);
                    }
                });

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("HAVE_FCNTL_H");
                    });
            }
        }
    }
}
