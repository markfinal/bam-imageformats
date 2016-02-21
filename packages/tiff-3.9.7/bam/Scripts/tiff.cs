#region License
// Copyright (c) 2010-2016, Mark Final
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of BuildAMation nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion // License
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
