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
namespace lpng
{
    namespace VisualC.WarningSuppression
    {
        sealed class PNGLibrary :
            C.SuppressWarningsDelegate
        {
            public PNGLibrary()
            {
                this.Add("png.c", "4996");
                this.Add("pnggccrd.c", "4206");
                this.Add("pngpread.c", "4267");
                this.Add("pngrtran.c", "4996");
                this.Add("pngrutil.c", "4267", "4310", "4996");
                this.Add("pngread.c", "4996");
                this.Add("pngset.c", "4267", "4996");
                this.Add("pngvcrd.c", "4206");
                this.Add("pngwio.c", "4267");
                this.Add("pngwrite.c", "4996");
                this.Add("pngwutil.c", "4267", "4996");
                this.Add("pngrutil.c", VisualCCommon.ToolchainVersion.VC2012, VisualCCommon.ToolchainVersion.VC2013, "4127");
            }
        }
    }

    namespace Gcc.WarningSuppression
    {
        sealed class PNGLibrary :
            C.SuppressWarningsDelegate
        {
            public PNGLibrary()
            {
                this.Add("png.c", "implicit-function-declaration");
                this.Add("pngread.c", "implicit-function-declaration");
                this.Add("pngrtran.c", "implicit-function-declaration");
                this.Add("pngrtran.c", GccCommon.ToolchainVersion.GCC_7, null, "implicit-fallthrough");
                this.Add("pngrutil.c", "implicit-function-declaration");
                this.Add("pngset.c", "implicit-function-declaration");
                this.Add("pngwrite.c", "implicit-function-declaration");
                this.Add("pngwutil.c", "implicit-function-declaration");
            }
        }
    }

    namespace Clang.WarningSuppression
    {
        sealed class PNGLibrary :
            C.SuppressWarningsDelegate
        {
            public PNGLibrary()
            {
                this.Add("pnggccrd.c", "empty-translation-unit");
                this.Add("pngvcrd.c", "empty-translation-unit");
            }
        }
    }
}
