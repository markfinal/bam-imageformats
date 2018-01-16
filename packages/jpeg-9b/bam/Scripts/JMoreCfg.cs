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
    class GenerateJMoreCfgHeader :
        C.ProceduralHeaderFile
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);
            this.Macros.Add("templateConfig", this.CreateTokenizedString("$(packagedir)/jmorecfg.h"));
        }

        protected override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/$(config)/PublicHeaders/jmorecfg.h");
            }
        }

        protected override string GuardString
        {
            get
            {
                return null;
            }
        }

        protected override string Contents
        {
            get
            {
                if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
                {
                    var contents = new System.Text.StringBuilder();
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.Macros["templateConfig"].ToString()))
                    {
                        for (;;)
                        {
                            var line = readFile.ReadLine();
                            if (null == line)
                            {
                                break;
                            }

                            // TODO: attempt at dynamic support, but not playing ball
#if false
                            if (line.StartsWith("#define GLOBAL(type)"))
                            {
                                line = "#define GLOBAL(type) __declspec(dllexport) type";
                            }
                            else if (line.StartsWith("#define EXTERN(type)"))
                            {
                                line = "#define EXTERN(type) extern __declspec(dllexport) type";
                            }
#endif

                            contents.AppendLine(line);
                        }
                    }
                    return contents.ToString();
                }
                else
                {
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.Macros["templateConfig"].ToString()))
                    {
                        return readFile.ReadToEnd();
                    }
                }
            }
        }
    }
}
