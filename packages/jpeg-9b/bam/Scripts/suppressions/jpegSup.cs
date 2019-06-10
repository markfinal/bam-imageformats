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
    namespace VisualC.WarningSuppression
    {
        sealed class JpegLibrary :
            C.SuppressWarningsDelegate
        {
            public JpegLibrary()
            {
                this.Add("jccoefct.c", "4100");
                this.Add("jccolor.c", "4100");
                this.Add("jcsample.c", "4100");
                this.Add("jctrans.c", "4100");
                this.Add("jdarith.c", "4100", "4244");
                this.Add("jdatadst.c", "4100", "4267");
                this.Add("jdatasrc.c", "4100");
                this.Add("jdcoefct.c", "4100");
                this.Add("jdcolor.c", "4100");
                this.Add("jerror.c", "4996");
                this.Add("jdhuff.c", "4244");
                this.Add("jdmarker.c", "4996");
                this.Add("jdmerge.c", "4100");
                this.Add("jdpostct.c", "4100");
                this.Add("jdsample.c", "4100");
                this.Add("jmemansi.c", "4100", "4996");
                this.Add("jmemmgr.c", "4267", "4127", "4996");
                this.Add("jquant1.c", "4100");
                this.Add("jquant2.c", "4100");
                this.Add("jdapimin.c", VisualCCommon.ToolchainVersion.VC2010, VisualCCommon.ToolchainVersion.VC2013, "4127");
                this.Add("jdmarker.c", VisualCCommon.ToolchainVersion.VC2010, VisualCCommon.ToolchainVersion.VC2013, "4127");
                this.Add("jquant1.c", VisualCCommon.ToolchainVersion.VC2010, VisualCCommon.ToolchainVersion.VC2013, "4127");
            }
        }
    }

    namespace Gcc.WarningSuppression
    {
        sealed class JpegLibrary :
            C.SuppressWarningsDelegate
        {
            public JpegLibrary()
            {
                this.Add("jccoefct.c", "unused-parameter");
                this.Add("jccolor.c", "unused-parameter");
                this.Add("jcmaster.c", GccCommon.ToolchainVersion.GCC_7, null, "implicit-fallthrough");
                this.Add("jcsample.c", "unused-parameter");
                this.Add("jctrans.c", "unused-parameter");
                this.Add("jdarith.c", "unused-parameter");
                this.Add("jdatadst.c", "unused-parameter");
                this.Add("jdatasrc.c", "unused-parameter");
                this.Add("jdcoefct.c", "unused-parameter");
                this.Add("jdcolor.c", "unused-parameter");
                this.Add("jdmerge.c", "unused-parameter");
                this.Add("jdpostct.c", "unused-parameter");
                this.Add("jdsample.c", "unused-parameter");
                this.Add("jmemansi.c", "unused-parameter");
                this.Add("jquant1.c", "unused-parameter");
                this.Add("jquant2.c", "unused-parameter");
            }
        }
    }

    namespace Clang.WarningSuppression
    {
        sealed class JpegLibrary :
            C.SuppressWarningsDelegate
        {
            public JpegLibrary()
            {
                this.Add("jccoefct.c", "unused-parameter");
                this.Add("jccolor.c", "unused-parameter");
                this.Add("jcsample.c", "unused-parameter");
                this.Add("jctrans.c", "unused-parameter");
                this.Add("jdarith.c", "unused-parameter");
                this.Add("jdatadst.c", "unused-parameter");
                this.Add("jdatasrc.c", "unused-parameter");
                this.Add("jdcoefct.c", "unused-parameter");
                this.Add("jdcolor.c", "unused-parameter");
                this.Add("jdmerge.c", "unused-parameter");
                this.Add("jdpostct.c", "unused-parameter");
                this.Add("jdsample.c", "unused-parameter");
                this.Add("jmemansi.c", "unused-parameter");
                this.Add("jquant1.c", "unused-parameter");
                this.Add("jquant2.c", "unused-parameter");
            }
        }
    }
}
