using Bam.Core;
namespace lpng
{
    [ModuleGroup("Thirdparty/libpng")]
    class GeneratePngConfHeader :
        C.ProceduralHeaderFile
    {
        protected override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders/pnglibconf.h");
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
                using (System.IO.TextReader readFile = new System.IO.StreamReader(this.CreateTokenizedString("$(packagedir)/scripts/pnglibconf.h.prebuilt").Parse()))
                {
                    return readFile.ReadToEnd();
                }
            }
        }
    }
}
