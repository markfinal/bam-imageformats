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
namespace jpeg
{
    [ModuleGroup("Thirdparty/libjpeg")]
    class JpegLibrary :
        C.StaticLibrary
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.SetSemanticVersion("9", "b", null);
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
                    var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                    if (null != clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                    }
                    var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                    if (null != gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jccoefct.c:346:51: error: unused parameter 'input_buf'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jccoefct.c:346:51: error: unused parameter 'input_buf'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jccolor.c:427:29: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jccolor.c:427:29: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jcsample.c:84:39: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jcsample.c:84:39: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jctrans.c:275:51: error: unused parameter 'input_buf'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jctrans.c:275:51: error: unused parameter 'input_buf'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdarith.c:753:31: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdarith.c:753:31: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdatadst.c:78:38: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdatadst.c:78:38: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdatasrc.c:57:35: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdatasrc.c:57:35: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdcoefct.c:230:38: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdcoefct.c:230:38: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdcolor.c:558:37: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdcolor.c:558:37: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdmerge.c:188:18: error: unused parameter 'in_row_groups_avail'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdmerge.c:188:18: error: unused parameter 'in_row_groups_avail'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdpostct.c:161:20: error: unused parameter 'output_buf'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdpostct.c:161:20: error: unused parameter 'output_buf'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdsample.c:92:19: error: unused parameter 'in_row_groups_avail'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jdsample.c:92:19: error: unused parameter 'in_row_groups_avail'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jmemansi.c:36:30: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jmemansi.c:36:30: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jquant1.c:246:32: error: unused parameter 'cinfo'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jquant1.c:246:32: error: unused parameter 'cinfo'
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
                        var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                        if (null != clangCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jquant2.c:226:16: error: unused parameter 'output_buf'
                        }
                        var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                        if (null != gccCompiler)
                        {
                            compiler.DisableWarnings.AddUnique("unused-parameter"); // jpeg-9b/jquant2.c:226:16: error: unused parameter 'output_buf'
                        }
                    }));
        }
    }
}
