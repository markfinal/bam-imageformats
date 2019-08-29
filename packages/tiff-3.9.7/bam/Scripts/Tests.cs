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
namespace tiff
{
    namespace tests
    {
        [Bam.Core.ModuleGroup("Thirdparty/tiff/tests")]
        class AsciiTagTest :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                this.Macros[Bam.Core.ModuleMacroNames.OutputName] = Bam.Core.TokenizedString.CreateVerbatim("ascii_tag");

                var source = this.CreateCSourceCollection("$(packagedir)/test/ascii_tag.c");
                this.CompileAndLinkAgainst<LibTiff_static>(source);

                source.PrivatePatch(settings =>
                    {
                        if (settings is ClangCommon.ICommonCompilerSettings)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("tautological-constant-out-of-range-compare"); // tiff-3.9.7/test/ascii_tag.c:128:41: error: comparison of constant 0 with boolean expression is always false [-Werror,-Wtautological-constant-out-of-range-compare]
                        }
                    });

                if (this.Linker is GccCommon.LinkerBase)
                {
                    this.PrivatePatch(settings =>
                        {
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.AddUnique("-lm");
                        });
                }
            }
        }

        [Bam.Core.ModuleGroup("Thirdparty/tiff/tests")]
        class LongTagTest :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                this.Macros[Bam.Core.ModuleMacroNames.OutputName] = Bam.Core.TokenizedString.CreateVerbatim("long_tag");

                var source = this.CreateCSourceCollection("$(packagedir)/test/long_tag.c");
                source.AddFiles("$(packagedir)/test/check_tag.c");
                this.CompileAndLinkAgainst<LibTiff_static>(source);

                source.PrivatePatch(settings =>
                    {
                        if (settings is ClangCommon.ICommonCompilerSettings)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("tautological-constant-out-of-range-compare"); // tiff-3.9.7/test/long_tag.c:112:41: error: comparison of constant 0 with boolean expression is always false [-Werror,-Wtautological-constant-out-of-range-compare]
                        }
                    });

                if (this.Linker is GccCommon.LinkerBase)
                {
                    this.PrivatePatch(settings =>
                        {
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.AddUnique("-lm");
                        });
                }
            }
        }

        [Bam.Core.ModuleGroup("Thirdparty/tiff/tests")]
        class ShortTagTest :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                this.Macros[Bam.Core.ModuleMacroNames.OutputName] = Bam.Core.TokenizedString.CreateVerbatim("short_tag");

                var source = this.CreateCSourceCollection("$(packagedir)/test/short_tag.c");
                source.AddFiles("$(packagedir)/test/check_tag.c");
                this.CompileAndLinkAgainst<LibTiff_static>(source);

                source.PrivatePatch(settings =>
                    {
                        if (settings is ClangCommon.ICommonCompilerSettings)
                        {
                            var compiler = settings as C.ICommonCompilerSettings;
                            compiler.DisableWarnings.AddUnique("tautological-constant-out-of-range-compare"); // tiff-3.9.7/test/short_tag.c:126:41: error: comparison of constant 0 with boolean expression is always false [-Werror,-Wtautological-constant-out-of-range-compare]
                        }
                    });

                if (this.Linker is GccCommon.LinkerBase)
                {
                    this.PrivatePatch(settings =>
                        {
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.AddUnique("-lm");
                        });
                }
            }
        }

        [Bam.Core.ModuleGroup("Thirdparty/tiff/tests")]
        class StripRwTest :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                this.Macros[Bam.Core.ModuleMacroNames.OutputName] = Bam.Core.TokenizedString.CreateVerbatim("strip_rw");

                this.CreateHeaderCollection("$(packagedir)/test/test_arrays.h");
                var source = this.CreateCSourceCollection("$(packagedir)/test/strip_rw.c");
                source.AddFiles("$(packagedir)/test/strip.c");
                source.AddFiles("$(packagedir)/test/test_arrays.c");
                this.CompileAndLinkAgainst<LibTiff_static>(source);

                if (this.Linker is GccCommon.LinkerBase)
                {
                    this.PrivatePatch(settings =>
                        {
                            var linker = settings as C.ICommonLinkerSettings;
                            linker.Libraries.AddUnique("-lm");
                        });
                }
            }
        }

        sealed class TiffTestRuntime :
            Publisher.Collation
        {
            protected override void
            Init()
            {
                base.Init();

                this.SetDefaultMacrosAndMappings(EPublishingType.ConsoleApplication);
                this.IncludeAllModulesInNamespace("tiff.tests", C.ConsoleApplication.ExecutableKey);
            }
        }
    }
}
