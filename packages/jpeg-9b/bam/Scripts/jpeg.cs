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
namespace jpeg
{
    [Bam.Core.ModuleGroup("Thirdparty/libjpeg")]
    class JpegLibrary :
        C.StaticLibrary
    {
        protected override void
        Init()
        {
            base.Init();

            this.SetSemanticVersion("9", "b", null);
            this.Macros[Bam.Core.ModuleMacroNames.OutputName] = this.CreateTokenizedString("jpeg");

            var source = this.CreateCSourceCollection("$(packagedir)/j*.c",
                filter: new System.Text.RegularExpressions.Regex(@"^((?!.*jmemname.c)(?!.*jmemnobs.c)(?!.*jmemdos.c)(?!.*jmemmac.c)(?!.*jpegtran.c).*)$"));

            if (source.Compiler is VisualCCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new VisualC.WarningSuppression.JpegLibrary());
            }
            else if (source.Compiler is GccCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Gcc.WarningSuppression.JpegLibrary());
            }
            else if (source.Compiler is ClangCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Clang.WarningSuppression.JpegLibrary());
            }

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Bam.Core.Graph.Instance.FindReferencedModule<CopyJpegStandardHeaders>();
            var generateConf = Bam.Core.Graph.Instance.FindReferencedModule<GenerateJConfigHeader>();
            var generateMoreCfg = Bam.Core.Graph.Instance.FindReferencedModule<GenerateJMoreCfgHeader>();
            source.DependsOn(copyStandardHeaders, generateConf, generateMoreCfg);

            // export the public headers
            this.UsePublicPatches(copyStandardHeaders);

            source.PrivatePatch(settings =>
                {
                    if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                    }
                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                    }
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
