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

            var generateConfig = Bam.Core.Graph.Instance.FindReferencedModule<GenerateConfigHeader>();
            source.DependsOn(generateConfig);
            source.UsePublicPatchesPrivately(generateConfig);
            var generateConf = Bam.Core.Graph.Instance.FindReferencedModule<GenerateConfHeader>();
            source.DependsOn(generateConf);
            source.UsePublicPatchesPrivately(generateConf);

            /*
            source.PrivatePatch(settings =>
                {
                    if (settings is C.ICOnlyCompilerSettings cCompiler)
                    {
                        cCompiler.LanguageStandard = C.ELanguageStandard.C99; // some C++ style comments
                    }
                });
                */

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                source.PrivatePatch(settings =>
                    {
                        /*
                        if (settings is C.ICommonPreprocessorSettings preprocessor)
                        {
                            preprocessor.PreprocessorDefines.Add("HAVE_FCNTL_H");
                            preprocessor.PreprocessorDefines.Add("USE_WIN32_FILEIO"); // see tiffio.h

                            // TODO: expose this as a configuration option
                            // the alternative is TIF_PLATFORM_WINDOWED
                            preprocessor.PreprocessorDefines.Add("TIF_PLATFORM_CONSOLE");
                        }
                        if (settings is C.ICommonCompilerSettingsWin winCompiler)
                        {
                            winCompiler.CharacterSet = C.ECharacterSet.NotSet;
                        }
                        */
                        if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                        {
                            vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                        }
                        if (settings is MingwCommon.ICommonCompilerSettings mingwCompiler)
                        {
                            mingwCompiler.AllWarnings = true;
                            mingwCompiler.ExtraWarnings = true;
                            mingwCompiler.Pedantic = true;
                        }
                    });

                this.PrivatePatch(settings =>
                    {
                        if (settings is VisualCCommon.ICommonLinkerSettings)
                        {
                            if (settings is C.ICommonLinkerSettings linker)
                            {
                                linker.Libraries.Add("USER32.lib");
                            }
                        }
                        if (settings is C.ICommonLinkerSettingsWin winLinker)
                        {
                            winLinker.ExportDefinitionFile = this.CreateTokenizedString("$(packagedir)/libtiff/libtiff.def");
                        }
                    });
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                source.PrivatePatch(settings =>
                    {
                        /*
                        // TODO: can this be less brute force?
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default;
                        */
                        if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                        {
                            clangCompiler.AllWarnings = true;
                            clangCompiler.ExtraWarnings = true;
                            clangCompiler.Pedantic = true;
                        }
                    });
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                source.PrivatePatch(settings =>
                    {
                        /*
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        gccCompiler.Visibility = GccCommon.EVisibility.Default;
                        */
                        if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                        {
                            gccCompiler.AllWarnings = true;
                            gccCompiler.ExtraWarnings = true;
                            gccCompiler.Pedantic = true;
                        }
                    });

                var versionScript = Bam.Core.Graph.Instance.FindReferencedModule<VersionScript>();
                this.DependsOn(versionScript);
                this.PrivatePatch(settings =>
                    {
                        if (settings is C.ICommonLinkerSettingsLinux linuxLinker)
                        {
                            linuxLinker.VersionScript = versionScript.InputPath;
                            linuxLinker.SharedObjectName = this.CreateTokenizedString("$(dynamicprefix)$(OutputName)$(sonameext)");
                        }

                        if (settings is C.ICommonLinkerSettings linker)
                        {
                            linker.Libraries.AddUnique("-lm");
                        }
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

            var generateConfig = Bam.Core.Graph.Instance.FindReferencedModule<GenerateConfigHeader>();
            source.DependsOn(generateConfig);
            source.UsePublicPatchesPrivately(generateConfig);
            var generateConf = Bam.Core.Graph.Instance.FindReferencedModule<GenerateConfHeader>();
            source.DependsOn(generateConf);
            source.UsePublicPatchesPrivately(generateConf);

            /*
            source.PrivatePatch(settings =>
                {
                    var cCompiler = settings as C.ICOnlyCompilerSettings;
                    cCompiler.LanguageStandard = C.ELanguageStandard.C99; // some C++ style comments
                });
                */

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                source.PrivatePatch(settings =>
                    {
                        /*
                        if (settings is C.ICommonPreprocessorSettings preprocessor)
                        {
                            preprocessor.PreprocessorDefines.Add("HAVE_FCNTL_H");
                            preprocessor.PreprocessorDefines.Add("USE_WIN32_FILEIO"); // see tiffio.h

                            // TODO: expose this as a configuration option
                            // the alternative is TIF_PLATFORM_WINDOWED
                            preprocessor.PreprocessorDefines.Add("TIF_PLATFORM_CONSOLE");
                        }
                        if (settings is C.ICommonCompilerSettingsWin winCompiler)
                        {
                            winCompiler.CharacterSet = C.ECharacterSet.NotSet;
                        }
                        */
                        if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                        {
                            vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                        }
                        if (settings is MingwCommon.ICommonCompilerSettings mingwCompiler)
                        {
                            mingwCompiler.AllWarnings = true;
                            mingwCompiler.ExtraWarnings = true;
                            mingwCompiler.Pedantic = true;
                        }
                    });
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                source.PrivatePatch(settings =>
                    {
                        if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                        {
                            clangCompiler.AllWarnings = true;
                            clangCompiler.ExtraWarnings = true;
                            clangCompiler.Pedantic = true;
                        }
                    });
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                source.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                        {
                            gccCompiler.AllWarnings = true;
                            gccCompiler.ExtraWarnings = true;
                            gccCompiler.Pedantic = true;
                        }
                    });
            }
        }
    }
}
