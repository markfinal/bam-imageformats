using Bam.Core;
namespace lpng
{
    [ModuleGroup("Thirdparty/libpng")]
    class PNGVersionScript :
        C.VersionScript
    {
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
                var pngh = this.CreateTokenizedString("$(packagedir)/png.h");

                var contents = new System.Text.StringBuilder();
                contents.AppendLine("PNG12_0"); // to match that from MakeFile.elf
                contents.AppendLine("{");
                contents.AppendLine("global:");
                using (System.IO.TextReader readFile = new System.IO.StreamReader(pngh.Parse()))
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
