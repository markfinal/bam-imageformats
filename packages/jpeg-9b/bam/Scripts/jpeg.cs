using Bam.Core;
namespace jpeg
{
    [ModuleGroup("Thirdparty/libjpeg")]
    sealed class JpegLibrary :
        C.StaticLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.Macros["MajorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("9");
            this.Macros["MinorVersion"] = Bam.Core.TokenizedString.CreateVerbatim("b");
            this.Macros["OutputName"] = this.CreateTokenizedString("jpeg");

            var source = this.CreateCSourceContainer("$(packagedir)/j*.c",
                filter: new System.Text.RegularExpressions.Regex(@"^((?!.*jmemname.c)(?!.*jmemnobs.c)(?!.*jmemdos.c)(?!.*jmemmac.c)(?!.*jpegtran.c).*)$"));

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Graph.Instance.FindReferencedModule<CopyJpegStandardHeaders>();
            var generateConf = Graph.Instance.FindReferencedModule<GenerateJConfigHeader>();
            var generateMoreCfg = Graph.Instance.FindReferencedModule<GenerateJMoreCfgHeader>();
            source.DependsOn(copyStandardHeaders, generateConf, generateMoreCfg);

            // export the public headers
            this.UsePublicPatches(copyStandardHeaders);

            source.PrivatePatch(settings =>
                {
                    var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                    }
                });

            source["jccoefct.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jccoefct.c(346): warning C4100: 'input_buf': unreferenced formal parameter
                        }
                    }));

            source["jccolor.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jccolor.c(427): warning C4100: 'cinfo': unreferenced formal parameter
                        }
                    }));

            source["jcsample.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jcsample.c(84): warning C4100: 'cinfo': unreferenced formal parameter
                        }
                    }));

            source["jctrans.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jctrans.c(275): warning C4100: 'input_buf': unreferenced formal parameter
                        }
                    }));

            source["jdarith.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4244"); // jpeg-9b\jdarith.c(480): warning C4244: '+=': conversion from 'int' to 'JCOEF', possible loss of data
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jdarith.c(753): warning C4100: 'cinfo': unreferenced formal parameter
                        }
                    }));

            source["jdatadst.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jdatadst.c(78): warning C4100: 'cinfo': unreferenced formal parameter
                            compiler.DisableWarnings.AddUnique("4267"); // jpeg-9b\jdatadst.c(185): warning C4267: '=': conversion from 'size_t' to 'unsigned long', possible loss of data
                        }
                    }));

            source["jdatasrc.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jdatasrc.c(57): warning C4100: 'cinfo': unreferenced formal parameter
                        }
                    }));

            source["jdcoefct.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jdcoefct.c(230): warning C4100: 'cinfo': unreferenced formal parameter
                        }
                    }));

            source["jdcolor.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jdcolor.c(558): warning C4100: 'cinfo': unreferenced formal parameter
                        }
                    }));

            source["jdhuff.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4244"); // jpeg-9b\jdhuff.c(999): warning C4244: '+=': conversion from 'int' to 'JCOEF', possible loss of data
                        }
                    }));

            source["jdmarker.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                        }
                    }));

            source["jdmerge.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jdmerge.c(188): warning C4100: 'in_row_groups_avail': unreferenced formal parameter
                        }
                    }));

            source["jdpostct.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jdpostct.c(162): warning C4100: 'out_rows_avail': unreferenced formal parameter
                        }
                    }));

            source["jdsample.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jdsample.c(92): warning C4100: 'in_row_groups_avail': unreferenced formal parameter
                        }
                    }));

            source["jerror.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                        }
                    }));

            source["jmemansi.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jmemansi.c(36): warning C4100: 'cinfo': unreferenced formal parameter
                        }
                    }));

            source["jmemmgr.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                            compiler.DisableWarnings.AddUnique("4267"); // jpeg-9b\jmemmgr.c(307): warning C4267: '+=': conversion from 'size_t' to 'long', possible loss of data
                            compiler.DisableWarnings.AddUnique("4127"); // jpeg-9b\jmemmgr.c(1045): warning C4127: conditional expression is constant
                        }
                    }));

#if false
            source["jpegtran.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.PreprocessorDefines.Add("_CRT_SECURE_NO_WARNINGS");
                            compiler.DisableWarnings.AddUnique("4702"); // jpeg-9b\jpegtran.c(576) : warning C4702: unreachable code
                        }
                    }));
#endif

            source["jquant1.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jquant1.c(246): warning C4100: 'ci': unreferenced formal parameter
                        }
                    }));

            source["jquant2.c"].ForEach(item =>
                item.PrivatePatch(settings =>
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                        if (null != vcCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("4100"); // jpeg-9b\jquant2.c(226): warning C4100: 'output_buf': unreferenced formal parameter
                        }
                    }));
        }
    }
}
