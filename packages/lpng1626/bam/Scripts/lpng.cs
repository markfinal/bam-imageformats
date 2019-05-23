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
            this.Macros["OutputName"] = this.CreateTokenizedString("png$(MajorVersion)$(MinorVersion)");

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                // to match that in the CMakeLists.txt
                this.Macros["sonameext"] = Bam.Core.TokenizedString.Create(".so.$(MajorVersion)$(MinorVersion)", null);
                this.Macros["dynamicext"] = Bam.Core.TokenizedString.Create(".so.$(MajorVersion)$(MinorVersion).$(PatchVersion).0", null);
            }

            var source = this.CreateCSourceContainer(
                "$(packagedir)/*.c",
                filter: new System.Text.RegularExpressions.Regex(@"^((?!.*example)(?!.*pngtest).*)$")
            );

            if (source.Compiler is VisualCCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new VisualC.WarningSuppression.PNGLibrary());
            }
            else if (source.Compiler is GccCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Gcc.WarningSuppression.PNGLibrary());
            }
            else if (source.Compiler is ClangCommon.CompilerBase)
            {
                source.SuppressWarningsDelegate(new Clang.WarningSuppression.PNGLibrary());
            }

            // note these dependencies are on SOURCE, as the headers are needed for compilation
            var copyStandardHeaders = Graph.Instance.FindReferencedModule<CopyPngStandardHeaders>();
            var generateConf = Graph.Instance.FindReferencedModule<GeneratePngConfHeader>();
            source.DependsOn(copyStandardHeaders, generateConf);

            // export the public headers
            this.UsePublicPatches(copyStandardHeaders);

            this.CompileAndLinkAgainst<zlib.ZLib>(source);

            source.PrivatePatch(settings =>
                {
                    var preprocessor = settings as C.ICommonPreprocessorSettings;

                    if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                        preprocessor.PreprocessorDefines.Add("PNG_BUILD_DLL");
                    }

                    if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;
                        gccCompiler.Visibility = GccCommon.EVisibility.Default;
                    }

                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                        clangCompiler.Visibility = ClangCommon.EVisibility.Default;
                    }

                    if (this.BuildEnvironment.Configuration == EConfiguration.Debug)
                    {
                        preprocessor.PreprocessorDefines.Add("PNG_DEBUG");
                    }
                });
        }
    }
}
