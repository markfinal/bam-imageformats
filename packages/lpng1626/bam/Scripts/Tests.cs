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
using System.Linq;
namespace lpng
{
    namespace tests
    {
        [ModuleGroup("Thirdparty/libpng/tests")]
        class PNGTest :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                var source = this.CreateCSourceCollection("$(packagedir)/pngtest.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                this.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonLinkerSettings gccLinker)
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
        class PNGValid :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                var source = this.CreateCSourceCollection("$(packagedir)/contrib/libtests/pngvalid.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                source.PrivatePatch(settings =>
                    {
                        var preprocessor = settings as C.ICommonPreprocessorSettings;
                        preprocessor.PreprocessorDefines.Add("PNG_FREESTANDING_TESTS");
                        preprocessor.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/contrib/visupng"));
                    });

                this.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonLinkerSettings gccLinker)
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
        class PNGstest :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                var source = this.CreateCSourceCollection("$(packagedir)/contrib/libtests/pngstest.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                source.PrivatePatch(settings =>
                    {
                        var preprocessor = settings as C.ICommonPreprocessorSettings;
                        preprocessor.PreprocessorDefines.Add("PNG_FREESTANDING_TESTS");
                        preprocessor.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/contrib/visupng"));
                    });

                this.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonLinkerSettings gccLinker)
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
        class PNGunknown :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                var source = this.CreateCSourceCollection("$(packagedir)/contrib/libtests/pngunknown.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                source.PrivatePatch(settings =>
                    {
                        var preprocessor = settings as C.ICommonPreprocessorSettings;
                        preprocessor.PreprocessorDefines.Add("PNG_FREESTANDING_TESTS");
                        preprocessor.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/contrib/visupng"));
                    });

                this.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonLinkerSettings gccLinker)
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
        class PNGimage :
            C.ConsoleApplication
        {
            protected override void
            Init()
            {
                base.Init();

                var source = this.CreateCSourceCollection("$(packagedir)/contrib/libtests/pngimage.c");
                this.CompileAndLinkAgainst<PNGLibrary>(source);
                this.CompileAndLinkAgainst<zlib.ZLib>(source);

                source.PrivatePatch(settings =>
                    {
                        var preprocessor = settings as C.ICommonPreprocessorSettings;
                        preprocessor.PreprocessorDefines.Add("PNG_FREESTANDING_TESTS");
                        preprocessor.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)/contrib/visupng"));
                    });

                this.PrivatePatch(settings =>
                    {
                        if (settings is GccCommon.ICommonLinkerSettings gccLinker)
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
            Init()
            {
                base.Init();

                this.SetDefaultMacrosAndMappings(EPublishingType.ConsoleApplication);
                this.IncludeAllModulesInNamespace("lpng.tests", C.ConsoleApplication.ExecutableKey);
                this.IncludeFiles<PNGTest>("$(packagedir)/pngtest.png", this.ExecutableDir, this.Find<PNGTest>().First());
            }
        }
    }
}
