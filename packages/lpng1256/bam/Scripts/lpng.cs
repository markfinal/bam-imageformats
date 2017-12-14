#region License
// Copyright (c) 2010-2017, Mark Final
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
namespace lpng
{
    [ModuleGroup("Thirdparty/libpng")]
    class PNGLibrary :
        C.DynamicLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                var versionScript = Bam.Core.Graph.Instance.FindReferencedModule<PNGVersionScript>();
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

            this.Macros["MajorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("1");
            this.Macros["MinorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("2");
            this.Macros["PatchVersion"] = Bam.Core.TokenizedString.CreateVerbatim("56");
            this.Macros["OutputName"] = this.CreateTokenizedString("png$(MajorVersion)$(MinorVersion)");

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                // to match that in the CMakeLists.txt
                this.Macros["sonameext"] = Bam.Core.TokenizedString.CreateVerbatim(".so.0");
                this.Macros["dynamicext"] = Bam.Core.TokenizedString.Create(".so.0.$(PatchVersion).0", null);
            }

            var source = this.CreateCSourceContainer("$(packagedir)/*.c", filter: new System.Text.RegularExpressions.Regex(@"^((?!.*example)(?!.*pngtest).*)$"));

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Graph.Instance.FindReferencedModule<CopyPngStandardHeaders>();
            source.DependsOn(copyStandardHeaders);

            // export the public headers
            this.UsePublicPatches(copyStandardHeaders);

            this.CompilePubliclyAndLinkAgainst<zlib.ZLib>(source); // png.h requires zlib.h

            this.PublicPatch((settings, appliedTo) =>
                {
                    var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != vcCompiler)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("PNG_DLL");
                    }
                });

            source.PrivatePatch(settings =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;

                    var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                        compiler.PreprocessorDefines.Add("PNG_BUILD_DLL");
                        compiler.PreprocessorDefines.Add("PNG_NO_MODULEDEF");
                    }

                    var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                    if (null != gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;
                        gccCompiler.Visibility = GccCommon.EVisibility.Default;
                    }

                    var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                    if (null != clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default;
                    }

                    if (this.BuildEnvironment.Configuration == EConfiguration.Debug)
                    {
                        compiler.PreprocessorDefines.Add("PNG_DEBUG");
                    }
                });

            source["png.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("implicit-function-declaration");
                            }
                        });
                });

            source["pnggccrd.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("4206"); // lpng1256\pnggccrd.c(27): warning C4206: nonstandard extension used: translation unit is empty
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                gccCompiler.Pedantic = false;
                            }
                            var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                            if (null != clangCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("empty-translation-unit"); // lpng1256/pnggccrd.c:26:7: error: ISO C requires a translation unit to contain at least one declaration
                            }
                        });
                });

            source["pngpread.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4267"); // lpng1256\pngpread.c(572): warning C4267: '-=': conversion from 'size_t' to 'png_uint_32', possible loss of data
                        }
                    });
                });

            source["pngread.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("implicit-function-declaration");
                            }
                        });
                });

            source["pngrtran.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("implicit-function-declaration");
                            }
                        });
                });

            source["pngrutil.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                                compiler.DisableWarnings.AddUnique("4267"); // lpng1256\pngrutil.c(228): warning C4267: '=': conversion from 'size_t' to 'uInt', possible loss of data
                                compiler.DisableWarnings.AddUnique("4310"); // lpng1256\pngrutil.c(1269): warning C4310: cast truncates constant value
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("implicit-function-declaration");
                            }
                        });
                });

            source["pngset.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                                compiler.DisableWarnings.AddUnique("4267"); // lpng1256\pngset.c(305): warning C4267: '=': conversion from 'size_t' to 'png_uint_32', possible loss of data
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("implicit-function-declaration");
                            }
                        });
                });

            source["pngvcrd.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("4206"); // lpng1256\pngvcrd.c(14): warning C4206: nonstandard extension used: translation unit is empty
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                gccCompiler.Pedantic = false;
                            }
                            var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                            if (null != clangCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("empty-translation-unit"); // lpng1256/pnggccrd.c:26:7: error: ISO C requires a translation unit to contain at least one declaration
                            }
                        });
                });

            source["pngwio.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4267"); // lpng1256\pngwio.c(60): warning C4267: '=': conversion from 'size_t' to 'png_uint_32', possible loss of data
                        }
                    });
                });

            source["pngwrite.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("implicit-function-declaration");
                            }
                        });
                });

            source["pngwutil.c"].ForEach(item =>
                {
                    item.PrivatePatch(settings =>
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                            if (null != vcCompiler)
                            {
                                compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                                compiler.DisableWarnings.AddUnique("4267"); // lpng1256\pngwutil.c(191): warning C4267: '=': conversion from 'size_t' to 'int', possible loss of data
                            }
                            var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                            if (null != gccCompiler)
                            {
                                compiler.DisableWarnings.AddUnique("implicit-function-declaration");
                            }
                        });
                });

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                this.LinkAgainst<WindowsSDK.WindowsSDK>();
            }
        }
    }

    namespace tests
    {
        [ModuleGroup("Thirdparty/libpng/tests")]
        sealed class PNGTest :
            C.ConsoleApplication
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var source = this.CreateCSourceContainer("$(packagedir)/pngtest.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
                {
                    this.LinkAgainst<WindowsSDK.WindowsSDK>();
                }

                this.PrivatePatch(settings =>
                    {
                        var gccLinker = settings as GccCommon.ICommonLinkerSettings;
                        if (null != gccLinker)
                        {
                            gccLinker.CanUseOrigin = true;
                            gccLinker.RPath.AddUnique("$ORIGIN");
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.AddUnique("-lm");
                        }
                    });
            }
        }

        sealed class PNGTestRuntime :
            Publisher.Collation
        {
#if D_NEW_PUBLISHING
#else
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var app = this.Include<PNGTest>(C.ConsoleApplication.Key, EPublishingType.ConsoleApplication, ".");

                this.Include<PNGLibrary>(C.DynamicLibrary.Key, ".", app);
                this.Include<zlib.ZLib>(C.DynamicLibrary.Key, ".", app);
                this.IncludeFile("$(packagedir)/pngtest.png", ".", app, false);
            }
#endif
        }
    }
}
