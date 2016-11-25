using Bam.Core;
namespace jpeg
{
    [ModuleGroup("Thirdparty/libjpeg")]
    class GenerateJConfigHeader :
        C.ProceduralHeaderFile
    {
        protected override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders/jconfig.h");
            }
        }

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
                string sourceHeaderPath = null;
                if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
                {
                    sourceHeaderPath = "$(packagedir)/jconfig.vc";
                }
                if (null != sourceHeaderPath)
                {
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.CreateTokenizedString(sourceHeaderPath).Parse()))
                    {
                        return readFile.ReadToEnd();
                    }
                }
                else
                {
                    var contents = new System.Text.StringBuilder();
                    contents.AppendLine("#define HAVE_PROTOTYPES");
                    contents.AppendLine("#define HAVE_UNSIGNED_CHAR");
                    contents.AppendLine("#define HAVE_UNSIGNED_SHORT");
                    contents.AppendLine("#define HAVE_STDDEF_H");
                    contents.AppendLine("#define HAVE_STDLIB_H");
                    return contents.ToString();
                }
            }
        }
    }
}
