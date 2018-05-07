#region License
// Copyright (c) 2010-2018, Mark Final
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

#if D_NEW_PUBLISHING
            var publishRoot = this.CreateTokenizedString("$(packagebuilddir)/$(config)/PublicHeaders");

            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (null != compiler)
                    {
                        compiler.IncludePaths.AddUnique(publishRoot);
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
#else
            // the build mode depends on whether this path has been set or not
            if (this.GeneratedPaths.ContainsKey(Key))
            {
                this.GeneratedPaths[Key].Aliased(this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders"));
            }
            else
            {
                this.RegisterGeneratedFile(Key, this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders"));
            }

            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (null != compiler)
                    {
                        compiler.IncludePaths.AddUnique(this.GeneratedPaths[Key]);
                    }
                });

            var tiffHeader = this.IncludeFile(this.CreateTokenizedString("$(packagedir)/libtiff/tiff.h"), ".");
            this.IncludeFile(this.CreateTokenizedString("$(packagedir)/libtiff/tiffvers.h"), ".", tiffHeader);
            this.IncludeFile(this.CreateTokenizedString("$(packagedir)/libtiff/tiffio.h"), ".", tiffHeader);
#endif
        }
    }

    [ModuleGroup("Thirdparty/tiff")]
    class GenerateConfHeader :
        C.ProceduralHeaderFile
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);
            if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
            {
                this.Macros.Add("templatetiffconf", this.CreateTokenizedString("$(packagedir)/libtiff/tiffconf.vc.h"));
            }
        }

        protected override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/$(config)/PublicHeaders/tiffconf.h");
            }
        }

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

    [ModuleGroup("Thirdparty/tiff")]
    class GenerateConfigHeader :
        C.ProceduralHeaderFile
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);
            if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
            {
                this.Macros.Add("templateConfig", this.CreateTokenizedString("$(packagedir)/libtiff/tif_config.vc.h"));
            }
        }

        protected override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/$(config)/PublicHeaders/tif_config.h");
            }
        }

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

    [ModuleGroup("Thirdparty/tiff")]
    class LibTiff :
        C.DynamicLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.Macros["OutputName"] = Bam.Core.TokenizedString.CreateVerbatim("tiff");
            this.SetSemanticVersion(3, 9, 7);

            var headers = this.CreateHeaderContainer("$(packagedir)/libtiff/*.h");
            var source = this.CreateCSourceContainer("$(packagedir)/libtiff/*.c", filter: new System.Text.RegularExpressions.Regex(@"^((?!.*acorn)(?!.*apple)(?!.*atari)(?!.*msdos)(?!.*unix)(?!.*win3).*)$"));
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

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Graph.Instance.FindReferencedModule<CopyStandardHeaders>();
            var generateConf = Graph.Instance.FindReferencedModule<GenerateConfHeader>();
            var generateConfig = Graph.Instance.FindReferencedModule<GenerateConfigHeader>();
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
                source["tif_dirinfo.c"].ForEach(item =>
                    {
                        item.PrivatePatch(settings =>
                            {
                                if (this.Linker is VisualCCommon.LinkerBase)
                                {
                                    var compiler = settings as C.ICommonCompilerSettings;
                                    compiler.DisableWarnings.AddUnique("4133"); // tiff-3.9.7\libtiff\tif_dirinfo.c(797): warning C4133: 'function' : incompatible types - from 'size_t *' to 'unsigned int *'
                                }
                            });
                    });
                source["tif_fax3.c"].ForEach(item =>
                    {
                        item.PrivatePatch(settings =>
                            {
                                if (this.Linker is VisualCCommon.LinkerBase)
                                {
                                    // VisualC 14.0
                                    var compiler = settings as C.ICommonCompilerSettings;
                                    compiler.DisableWarnings.AddUnique("4311"); // tiff-3.9.7\libtiff\tif_fax3.c(395): warning C4311: 'type cast': pointer truncation from 'unsigned char *' to 'unsigned long'
                                }
                            });
                    });
                source["tif_win32.c"].ForEach(item =>
                    {
                        item.PrivatePatch(settings =>
                            {
                                if (this.Linker is VisualCCommon.LinkerBase)
                                {
                                    // VisualC 14.0
                                    var compiler = settings as C.ICommonCompilerSettings;
                                    compiler.DisableWarnings.AddUnique("4311"); // tiff-3.9.7\libtiff\tif_win32.c(212): warning C4311: 'type cast': pointer truncation from 'thandle_t' to 'int'
                                    compiler.DisableWarnings.AddUnique("4312"); // tiff-3.9.7\libtiff\tif_win32.c(156): warning C4312: 'type cast': conversion from 'int' to 'thandle_t' of greater size
                                }
                            });
                    });
                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("HAVE_FCNTL_H");
                        compiler.PreprocessorDefines.Add("USE_WIN32_FILEIO"); // see tiffio.h

                        // TODO: expose this as a configuration option
                        // the alternative is TIF_PLATFORM_WINDOWED
                        compiler.PreprocessorDefines.Add("TIF_PLATFORM_CONSOLE");

                        var winCompiler = settings as C.ICommonCompilerSettingsWin;
                        winCompiler.CharacterSet = C.ECharacterSet.NotSet;

                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level2;
                        }

                        var mingwCompiler = settings as MingwCommon.ICommonCompilerSettings;
                        if (null != mingwCompiler)
                        {
                            mingwCompiler.AllWarnings = false;
                            mingwCompiler.ExtraWarnings = false;
                            mingwCompiler.Pedantic = true;
                        }
                    });

                this.PrivatePatch(settings =>
                    {
                        if (this.Linker is VisualCCommon.LinkerBase)
                        {
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.Add("USER32.lib");
                        }

                        var winLinker = settings as C.ICommonLinkerSettingsWin;
                        winLinker.ExportDefinitionFile = this.CreateTokenizedString("$(packagedir)/libtiff/libtiff.def");
                    });

                if (this.Linker is VisualCCommon.LinkerBase)
                {
                    this.CompilePubliclyAndLinkAgainst<WindowsSDK.WindowsSDK>(source);
                }
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

                source["tif_lzw.c"].ForEach(item =>
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            if (null != compiler)
                            {
                                compiler.DisableWarnings.AddUnique("unused-parameter"); // tiff-3.9.7/libtiff/tif_lzw.c:1060:28: error: unused parameter 'scheme' [-Werror,-Wunused-parameter]
                            }
                        }));

                source["tif_print.c"].ForEach(item =>
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            if (null != compiler)
                            {
                                compiler.DisableWarnings.AddUnique("unused-variable"); // tiff-3.9.7/libtiff/tif_print.c:118:17: error: unused variable 'td' [-Werror,-Wunused-variable]
                            }
                        }));

                source["tif_write.c"].ForEach(item =>
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            if (null != compiler)
                            {
                                compiler.DisableWarnings.AddUnique("sign-compare"); // tiff-3.9.7/libtiff/tif_write.c:633:49: error: comparison of integers of different signs: 'toff_t' (aka 'unsigned int') and 'tsize_t' (aka 'int') [-Werror,-Wsign-compare]
                            }
                        }));
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
                        if (null != gccLinker)
                        {
                            gccLinker.VersionScript = versionScript.InputPath;
                        }
                    });
            }
        }
    }

    // Dynamic library exports are via .def file (Windows), and .map file (Linux) [brute force on OSX]
    // however, sometimes a static library is preferred
    [ModuleGroup("Thirdparty/tiff")]
    class LibTiff_static :
        C.StaticLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.Macros["OutputName"] = Bam.Core.TokenizedString.CreateVerbatim("tiff");
            this.SetSemanticVersion(3, 9, 7);

            var headers = this.CreateHeaderContainer("$(packagedir)/libtiff/*.h");
            var source = this.CreateCSourceContainer("$(packagedir)/libtiff/*.c", filter: new System.Text.RegularExpressions.Regex(@"^((?!.*acorn)(?!.*apple)(?!.*atari)(?!.*msdos)(?!.*unix)(?!.*win3).*)$"));
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

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Graph.Instance.FindReferencedModule<CopyStandardHeaders>();
            var generateConf = Graph.Instance.FindReferencedModule<GenerateConfHeader>();
            var generateConfig = Graph.Instance.FindReferencedModule<GenerateConfigHeader>();
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
                source["tif_dirinfo.c"].ForEach(item =>
                    {
                        item.PrivatePatch(settings =>
                            {
                                if (item.Compiler is VisualCCommon.CompilerBase)
                                {
                                    var compiler = settings as C.ICommonCompilerSettings;
                                    compiler.DisableWarnings.AddUnique("4133"); // tiff-3.9.7\libtiff\tif_dirinfo.c(797): warning C4133: 'function' : incompatible types - from 'size_t *' to 'unsigned int *'
                                }
                            });
                    });
                source["tif_fax3.c"].ForEach(item =>
                    {
                        item.PrivatePatch(settings =>
                            {
                                if (item.Compiler is VisualCCommon.CompilerBase)
                                {
                                    // VisualC 14.0
                                    var compiler = settings as C.ICommonCompilerSettings;
                                    compiler.DisableWarnings.AddUnique("4311"); // tiff-3.9.7\libtiff\tif_fax3.c(395): warning C4311: 'type cast': pointer truncation from 'unsigned char *' to 'unsigned long'
                                }
                            });
                    });
                source["tif_win32.c"].ForEach(item =>
                    {
                        item.PrivatePatch(settings =>
                            {
                                if (item.Compiler is VisualCCommon.CompilerBase)
                                {
                                    // VisualC 14.0
                                    var compiler = settings as C.ICommonCompilerSettings;
                                    compiler.DisableWarnings.AddUnique("4311"); // tiff-3.9.7\libtiff\tif_win32.c(212): warning C4311: 'type cast': pointer truncation from 'thandle_t' to 'int'
                                    compiler.DisableWarnings.AddUnique("4312"); // tiff-3.9.7\libtiff\tif_win32.c(156): warning C4312: 'type cast': conversion from 'int' to 'thandle_t' of greater size
                                }
                            });
                    });
                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("HAVE_FCNTL_H");
                        compiler.PreprocessorDefines.Add("USE_WIN32_FILEIO"); // see tiffio.h

                        // TODO: expose this as a configuration option
                        // the alternative is TIF_PLATFORM_WINDOWED
                        compiler.PreprocessorDefines.Add("TIF_PLATFORM_CONSOLE");

                        var winCompiler = settings as C.ICommonCompilerSettingsWin;
                        winCompiler.CharacterSet = C.ECharacterSet.NotSet;

                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level2;
                        }

                        var mingwCompiler = settings as MingwCommon.ICommonCompilerSettings;
                        if (null != mingwCompiler)
                        {
                            mingwCompiler.AllWarnings = false;
                            mingwCompiler.ExtraWarnings = false;
                            mingwCompiler.Pedantic = true;
                        }
                    });
                if (this.Librarian is VisualCCommon.LibrarianBase)
                {
                    this.CompileAgainst<WindowsSDK.WindowsSDK>(source);
                }
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

                source["tif_lzw.c"].ForEach(item =>
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            if (null != compiler)
                            {
                                compiler.DisableWarnings.AddUnique("unused-parameter"); // tiff-3.9.7/libtiff/tif_lzw.c:1060:28: error: unused parameter 'scheme' [-Werror,-Wunused-parameter]
                            }
                        }));

                source["tif_print.c"].ForEach(item =>
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            if (null != compiler)
                            {
                                compiler.DisableWarnings.AddUnique("unused-variable"); // tiff-3.9.7/libtiff/tif_print.c:118:17: error: unused variable 'td' [-Werror,-Wunused-variable]
                            }
                        }));

                source["tif_write.c"].ForEach(item =>
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            if (null != compiler)
                            {
                                compiler.DisableWarnings.AddUnique("sign-compare"); // tiff-3.9.7/libtiff/tif_write.c:633:49: error: comparison of integers of different signs: 'toff_t' (aka 'unsigned int') and 'tsize_t' (aka 'int') [-Werror,-Wsign-compare]
                            }
                        }));
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
