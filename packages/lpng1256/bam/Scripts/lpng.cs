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

            this.Macros["MajorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("1");
            this.Macros["MinorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("2");
            this.Macros["PatchVersion"] = Bam.Core.TokenizedString.CreateVerbatim("56");
            this.Macros["OutputName"] = this.CreateTokenizedString("png$(MajorVersion)$(MinorVersion)");

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                // to match that in the CMakeLists.txt
                this.Macros["sonameext"] = Bam.Core.TokenizedString.CreateInline(".so.0");
                this.Macros["dynamicext"] = Bam.Core.TokenizedString.CreateInline(".so.0.$(PatchVersion).0");
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
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var app = this.Include<PNGTest>(C.ConsoleApplication.Key, EPublishingType.ConsoleApplication, ".");

                this.Include<PNGLibrary>(C.DynamicLibrary.Key, ".", app);
                this.Include<zlib.ZLib>(C.DynamicLibrary.Key, ".", app);
            }
        }
    }
}
