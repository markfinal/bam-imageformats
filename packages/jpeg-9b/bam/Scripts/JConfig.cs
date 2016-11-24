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
                else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
                {
                    sourceHeaderPath = "$(packagedir)/jconfig.mac";
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
                    return "";
                }
            }
        }
    }
}
