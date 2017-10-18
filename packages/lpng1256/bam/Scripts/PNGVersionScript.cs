#region License
// Copyright (c) 2010-2017, Mark Final
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
    class PNGVersionScript :
        C.VersionScript
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);
            this.Macros.Add("templateConfig", this.CreateTokenizedString("$(packagedir)/png.h"));
        }

        public override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/$(config)/png12.map");
            }
        }

        protected override string Contents
        {
            get
            {
                var exportRegEx = new System.Text.RegularExpressions.Regex(@"PNG_EXPORT\([A-za-z_0-9]*,([A-za-z_0-9]*)\)");

                var contents = new System.Text.StringBuilder();
                contents.AppendLine("PNG12_0"); // to match that from MakeFile.elf
                contents.AppendLine("{");
                contents.AppendLine("global:");
                using (System.IO.TextReader readFile = new System.IO.StreamReader(this.Macros["templateConfig"].ToString()))
                {
                    for (;;)
                    {
                        var line = readFile.ReadLine();
                        if (null == line)
                        {
                            break;
                        }
                        var matches = exportRegEx.Matches(line);
                        if (matches.Count > 0)
                        {
                            Bam.Core.Log.DebugMessage("Found {0} matches on {1}", matches.Count, line);
                            foreach (System.Text.RegularExpressions.Match match in matches)
                            {
                                Bam.Core.Log.DebugMessage("\t{0} captures", match.Captures.Count);
                                foreach (System.Text.RegularExpressions.Capture capture in match.Captures)
                                {
                                    Bam.Core.Log.DebugMessage("\t\t{0}", capture.Value);
                                }
                                Bam.Core.Log.DebugMessage("\t{0} groups", match.Groups.Count);
                                if (match.Groups[0].Success)
                                {
                                    Bam.Core.Log.DebugMessage("\t\t{0}", match.Groups[1].Value);
                                    contents.AppendFormat("{0};", match.Groups[1].Value);
                                    contents.AppendLine();
                                }
                            }
                        }
                    }
                }
                contents.AppendLine("};");
                return contents.ToString();
            }
        }
    }
}
