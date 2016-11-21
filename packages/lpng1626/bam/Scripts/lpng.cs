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
            this.Macros["MinorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("6");
            this.Macros["PatchVersion"] = Bam.Core.TokenizedString.CreateVerbatim("26");
            this.Macros["OutputName"] = Bam.Core.TokenizedString.CreateVerbatim("png");

            var source = this.CreateCSourceContainer("$(packagedir)/*.c", filter: new System.Text.RegularExpressions.Regex(@"^((?!.*example)(?!.*pngtest).*)$"));

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Graph.Instance.FindReferencedModule<CopyPngStandardHeaders>();
            var generateConf = Graph.Instance.FindReferencedModule<GeneratePngConfHeader>();
            source.DependsOn(copyStandardHeaders, generateConf);

            // export the public headers
            this.UsePublicPatches(copyStandardHeaders);

            this.CompileAndLinkAgainst<zlib.ZLib>(source);

            source.PrivatePatch(settings =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;

                    var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                        compiler.PreprocessorDefines.Add("PNG_BUILD_DLL");
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

        [ModuleGroup("Thirdparty/libpng/tests")]
        sealed class PNGValid :
            C.ConsoleApplication
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var source = this.CreateCSourceContainer("$(packagedir)/contrib/libtests/pngvalid.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("PNG_FREESTANDING_TESTS");
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/contrib/visupng"));
                    });

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

        [ModuleGroup("Thirdparty/libpng/tests")]
        sealed class PNGstest :
            C.ConsoleApplication
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var source = this.CreateCSourceContainer("$(packagedir)/contrib/libtests/pngstest.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("PNG_FREESTANDING_TESTS");
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/contrib/visupng"));
                    });

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

        [ModuleGroup("Thirdparty/libpng/tests")]
        sealed class PNGunknown :
            C.ConsoleApplication
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var source = this.CreateCSourceContainer("$(packagedir)/contrib/libtests/pngunknown.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("PNG_FREESTANDING_TESTS");
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/contrib/visupng"));
                    });

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

        [ModuleGroup("Thirdparty/libpng/tests")]
        sealed class PNGimage :
            C.ConsoleApplication
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                var source = this.CreateCSourceContainer("$(packagedir)/contrib/libtests/pngimage.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                source.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.PreprocessorDefines.Add("PNG_FREESTANDING_TESTS");
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/contrib/visupng"));
                    });

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
                this.Include<PNGValid>(C.ConsoleApplication.Key, ".", app);
                this.Include<PNGstest>(C.ConsoleApplication.Key, ".", app);
                this.Include<PNGunknown>(C.ConsoleApplication.Key, ".", app);
                this.Include<PNGimage>(C.ConsoleApplication.Key, ".", app);

                this.Include<PNGLibrary>(C.DynamicLibrary.Key, ".", app);
                this.Include<zlib.ZLib>(C.DynamicLibrary.Key, ".", app);
            }
        }
    }
}
