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
namespace tiff
{
    [Bam.Core.ModuleGroup("Thirdparty/tiff")]
    class GenerateConfigHeader :
        C.ProceduralHeaderFile
    {
        protected override void
        Init()
        {
            base.Init();
            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                this.Macros.Add("templateConfig", this.CreateTokenizedString("$(packagedir)/libtiff/tif_config.vc.h"));
            }
        }

        protected override Bam.Core.TokenizedString OutputPath => this.CreateTokenizedString("$(packagebuilddir)/$(moduleoutputdir)/tif_config.h");

        protected override string GuardString
        {
            get
            {
                if (Bam.Core.OSUtilities.IsWindowsHosting)
                {
                    return null;
                }
                return base.GuardString;
            }
        }

        protected override string Contents
        {
            get
            {
                if (this.BuildEnvironment.Platform.Includes(EPlatform.Windows))
                {
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.Macros.FromName("templateConfig").ToString()))
                    {
                        return readFile.ReadToEnd();
                    }
                }
                else
                {
                    var contents = new System.Text.StringBuilder();
                    contents.AppendLine("#define HAVE_SEARCH_H");
                    contents.AppendLine("#define TIFF_INT64_T long long");
                    contents.AppendLine("#define TIFF_UINT64_T unsigned long long");
                    contents.AppendLine("#define HAVE_FCNTL_H");
                    contents.AppendLine("#define HAVE_STRING_H");
                    contents.AppendLine("#define HOST_FILLORDER FILLORDER_LSB2MSB");
                    contents.AppendLine("#define HAVE_IEEEFP 1");
                    contents.AppendLine("#define HAVE_UNISTD_H 1");
                    if (this.BuildEnvironment.Platform.Includes(EPlatform.OSX))
                    {
                        contents.AppendLine("#define HAVE_GETOPT 1");
                    }
                    return contents.ToString();
                }
            }
        }
    }
}
