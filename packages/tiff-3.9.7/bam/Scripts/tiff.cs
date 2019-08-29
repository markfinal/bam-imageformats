#region License
// Copyright (c) 2010-2019, Mark Final
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
    [Bam.Core.ModuleGroup("Thirdparty/tiff")]
    class CopyStandardHeaders :
        Publisher.Collation
    {
        protected override void
        Init()
        {
            base.Init();

            var publishRoot = this.CreateTokenizedString("$(packagebuilddir)/$(config)/PublicHeaders");

            this.PublicPatch((settings, appliedTo) =>
                {
                    if (settings is C.ICommonPreprocessorSettings preprocessor)
                    {
                        preprocessor.IncludePaths.AddUnique(publishRoot);
                    }
                });

            var headerPaths = new Bam.Core.StringArray
            {
                "tiff.h",
                "tiffvers.h",
                "tiffio.h"
            };

            foreach (var header in headerPaths)
            {
                this.IncludeFiles<CopyStandardHeaders>("$(packagedir)/libtiff/" + header, publishRoot, null);
            }
        }
    }

    [Bam.Core.ModuleGroup("Thirdparty/tiff")]
    class GenerateConfHeader :
        C.ProceduralHeaderFile
    {
        protected override void
        Init()
        {
            base.Init();
            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                this.Macros.Add("templatetiffconf", this.CreateTokenizedString("$(packagedir)/libtiff/tiffconf.vc.h"));
            }
        }

        protected override Bam.Core.TokenizedString OutputPath => this.CreateTokenizedString("$(packagebuilddir)/$(config)/PublicHeaders/tiffconf.h");

        protected override string GuardString
        {
            get
            {
                if (Bam.Core.OSUtilities.IsWindowsHosting)
                {
                    return null;
                }
                return base.GuardString;
            }
        }

        protected override string Contents
        {
            get
            {
                if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
                {
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.Macros["templatetiffconf"].ToString()))
                    {
                        return readFile.ReadToEnd();
                    }
                }
                else
                {
                    var contents = new System.Text.StringBuilder();
                    contents.AppendLine("#define SIZEOF_INT 4");
                    contents.AppendLine("#define LZW_SUPPORT 1");
                    return contents.ToString();
                }
            }
        }
    }

    [Bam.Core.ModuleGroup("Thirdparty/tiff")]
    class GenerateConfigHeader :
        C.ProceduralHeaderFile
    {
        protected override void
        Init()
        {
            base.Init();
            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                this.Macros.Add("templateConfig", this.CreateTokenizedString("$(packagedir)/libtiff/tif_config.vc.h"));
            }
        }

        protected override Bam.Core.TokenizedString OutputPath => this.CreateTokenizedString("$(packagebuilddir)/$(config)/PublicHeaders/tif_config.h");

        protected override string GuardString
        {
            get
            {
                if (Bam.Core.OSUtilities.IsWindowsHosting)
                {
                    return null;
                }
                return base.GuardString;
            }
        }

        protected override string Contents
        {
            get
            {
                if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
                {
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.Macros["templateConfig"].ToString()))
                    {
                        return readFile.ReadToEnd();
                    }
                }
                else
                {
                    var contents = new System.Text.StringBuilder();
                    contents.AppendLine("#define HAVE_SEARCH_H");
                    contents.AppendLine("#define TIFF_INT64_T long long");
                    contents.AppendLine("#define TIFF_UINT64_T unsigned long long");
                    contents.AppendLine("#define HAVE_FCNTL_H");
                    contents.AppendLine("#define HAVE_STRING_H");
                    contents.AppendLine("#define HOST_FILLORDER FILLORDER_LSB2MSB");
                    contents.AppendLine("#define HAVE_IEEEFP 1");
                    contents.AppendLine("#define HAVE_UNISTD_H 1");
                    if (this.BuildEnvironment.Platform.Includes(EPlatform.OSX))
                    {
                        contents.AppendLine("#define HAVE_GETOPT 1");
                    }
                    return contents.ToString();
                }
            }
        }
    }

    [Bam.Core.ModuleGroup("Thirdparty/tiff")]
    class LibTiff :
        C.DynamicLibrary
    {
        protected override void
        Init()
        {
            base.Init();

            this.Macros[Bam.Core.ModuleMacroNames.OutputName] = Bam.Core.TokenizedString.CreateVerbatim("tiff");
            this.SetSemanticVersion(3, 9, 7);

            var headers = this.CreateHeaderCollection("$(packagedir)/libtiff/*.h");
            var source = this.CreateCSourceCollection("$(packagedir)/libtiff/*.c", filter: new System.Text.RegularExpressions.Regex(@"^((?!.*acorn)(?!.*apple)(?!.*atari)(?!.*msdos)(?!.*unix)(?!.*win3).*)$"));
            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                headers.AddFiles("$(packagedir)/port/*.h");
                source.AddFiles("$(packagedir)/libtiff/tif_win32.c");
                source.AddFiles("$(packagedir)/port/getopt.c");
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                source.AddFiles("$(packagedir)/libtiff/tif_unix.c");
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                source.AddFiles("$(packagedir)/libtiff/tif_unix.c");
            }

            if (source.Compiler is VisualCCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new VisualC.WarningSuppression.LibTiff());
            }
            else if (source.Compiler is GccCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Gcc.WarningSuppression.LibTiff());
            }
            else if (source.Compiler is ClangCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Clang.WarningSuppression.LibTiff());
            }

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Bam.Core.Graph.Instance.FindReferencedModule<CopyStandardHeaders>();
            var generateConf = Bam.Core.Graph.Instance.FindReferencedModule<GenerateConfHeader>();
            var generateConfig = Bam.Core.Graph.Instance.FindReferencedModule<GenerateConfigHeader>();
            source.DependsOn(copyStandardHeaders, generateConf, generateConfig);

            // export the public headers
            this.UsePublicPatches(copyStandardHeaders);

            source.PrivatePatch(settings =>
                {
                    var cCompiler = settings as C.ICOnlyCompilerSettings;
                    cCompiler.LanguageStandard = C.ELanguageStandard.C99; // some C++ style comments
                });

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                source.PrivatePatch(settings =>
                    {
                        var preprocessor = settings as C.ICommonPreprocessorSettings;
                        preprocessor.PreprocessorDefines.Add("HAVE_FCNTL_H");
                        preprocessor.PreprocessorDefines.Add("USE_WIN32_FILEIO"); // see tiffio.h

                        // TODO: expose this as a configuration option
                        // the alternative is TIF_PLATFORM_WINDOWED
                        preprocessor.PreprocessorDefines.Add("TIF_PLATFORM_CONSOLE");

                        var winCompiler = settings as C.ICommonCompilerSettingsWin;
                        winCompiler.CharacterSet = C.ECharacterSet.NotSet;

                        if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                        {
                            vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level2;
                        }

                        if (settings is MingwCommon.ICommonCompilerSettings mingwCompiler)
                        {
                            mingwCompiler.AllWarnings = false;
                            mingwCompiler.ExtraWarnings = false;
                            mingwCompiler.Pedantic = true;
                        }
                    });

                this.PrivatePatch(settings =>
                    {
                        if (settings is VisualCCommon.ICommonLinkerSettings)
                        {
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.Add("USER32.lib");
                        }

                        var winLinker = settings as C.ICommonLinkerSettingsWin;
                        winLinker.ExportDefinitionFile = this.CreateTokenizedString("$(packagedir)/libtiff/libtiff.def");
                    });
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("int-to-void-pointer-cast");

                        // TODO: can this be less brute force?
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default;

                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                    });
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("pointer-to-int-cast");
                        compiler.DisableWarnings.AddUnique("int-to-pointer-cast");

                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        gccCompiler.Visibility = GccCommon.EVisibility.Default;

                        gccCompiler.AllWarnings = false;
                        gccCompiler.ExtraWarnings = false;
                        gccCompiler.Pedantic = true;
                    });

                var versionScript = Bam.Core.Graph.Instance.FindReferencedModule<VersionScript>();
                this.DependsOn(versionScript);
                this.PrivatePatch(settings =>
                    {
                        var gccLinker = settings as GccCommon.ICommonLinkerSettings;
                        gccLinker.VersionScript = versionScript.InputPath;
                    });
            }
        }
    }

    // Dynamic library exports are via .def file (Windows), and .map file (Linux) [brute force on OSX]
    // however, sometimes a static library is preferred
    [Bam.Core.ModuleGroup("Thirdparty/tiff")]
    class LibTiff_static :
        C.StaticLibrary
    {
        protected override void
        Init()
        {
            base.Init();

            this.Macros[Bam.Core.ModuleMacroNames.OutputName] = Bam.Core.TokenizedString.CreateVerbatim("tiff");
            this.SetSemanticVersion(3, 9, 7);

            var headers = this.CreateHeaderCollection("$(packagedir)/libtiff/*.h");
            var source = this.CreateCSourceCollection("$(packagedir)/libtiff/*.c", filter: new System.Text.RegularExpressions.Regex(@"^((?!.*acorn)(?!.*apple)(?!.*atari)(?!.*msdos)(?!.*unix)(?!.*win3).*)$"));
            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                headers.AddFiles("$(packagedir)/port/*.h");
                source.AddFiles("$(packagedir)/libtiff/tif_win32.c");
                source.AddFiles("$(packagedir)/port/getopt.c");
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                source.AddFiles("$(packagedir)/libtiff/tif_unix.c");
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                source.AddFiles("$(packagedir)/libtiff/tif_unix.c");
            }

            if (source.Compiler is VisualCCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new VisualC.WarningSuppression.LibTiff());
            }
            else if (source.Compiler is GccCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Gcc.WarningSuppression.LibTiff());
            }
            else if (source.Compiler is ClangCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Clang.WarningSuppression.LibTiff());
            }

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Bam.Core.Graph.Instance.FindReferencedModule<CopyStandardHeaders>();
            var generateConf = Bam.Core.Graph.Instance.FindReferencedModule<GenerateConfHeader>();
            var generateConfig = Bam.Core.Graph.Instance.FindReferencedModule<GenerateConfigHeader>();
            source.DependsOn(copyStandardHeaders, generateConf, generateConfig);

            // export the public headers
            this.UsePublicPatches(copyStandardHeaders);

            source.PrivatePatch(settings =>
                {
                    var cCompiler = settings as C.ICOnlyCompilerSettings;
                    cCompiler.LanguageStandard = C.ELanguageStandard.C99; // some C++ style comments
                });

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                source.PrivatePatch(settings =>
                    {
                        var preprocessor = settings as C.ICommonPreprocessorSettings;
                        preprocessor.PreprocessorDefines.Add("HAVE_FCNTL_H");
                        preprocessor.PreprocessorDefines.Add("USE_WIN32_FILEIO"); // see tiffio.h

                        // TODO: expose this as a configuration option
                        // the alternative is TIF_PLATFORM_WINDOWED
                        preprocessor.PreprocessorDefines.Add("TIF_PLATFORM_CONSOLE");

                        var winCompiler = settings as C.ICommonCompilerSettingsWin;
                        winCompiler.CharacterSet = C.ECharacterSet.NotSet;

                        if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                        {
                            vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level2;
                        }

                        if (settings is MingwCommon.ICommonCompilerSettings mingwCompiler)
                        {
                            mingwCompiler.AllWarnings = false;
                            mingwCompiler.ExtraWarnings = false;
                            mingwCompiler.Pedantic = true;
                        }
                    });
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("int-to-void-pointer-cast");

                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                    });
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("pointer-to-int-cast");
                        compiler.DisableWarnings.AddUnique("int-to-pointer-cast");

                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        gccCompiler.AllWarnings = false;
                        gccCompiler.ExtraWarnings = false;
                        gccCompiler.Pedantic = true;
                    });
            }
        }
    }
}
