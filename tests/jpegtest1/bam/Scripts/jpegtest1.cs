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
namespace jpegtest1
{
    sealed class JPEGTest1 :
        C.ConsoleApplication
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.CreateHeaderContainer("$(packagedir)/source/*.h");
            var source = this.CreateCSourceContainer("$(packagedir)/source/*.c");
            this.CompileAndLinkAgainst<jpeg.JpegLibrary>(source);

            if (this.Linker is VisualCCommon.LinkerBase)
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

    sealed class JPEGTest1Runtime :
        Publisher.Collation
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

#if D_NEW_PUBLISHING
            this.SetDefaultMacrosAndMappings(EPublishingType.ConsoleApplication);
            this.Include<JPEGTest1>(C.ConsoleApplication.Key);
#else
            /*var app = */this.Include<JPEGTest1>(C.ConsoleApplication.Key, EPublishingType.ConsoleApplication, ".");
#endif
        }
    }
}
