using Bam.Core;
namespace jpeg
{
    [ModuleGroup("Thirdparty/libjpeg")]
    class GenerateJMoreCfgHeader :
        C.ProceduralHeaderFile
    {
        protected override TokenizedString OutputPath
        {
            get
            {
                return this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders/jmorecfg.h");
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
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.CreateTokenizedString("$(packagedir)/jmorecfg.h").Parse()))
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
                    using (System.IO.TextReader readFile = new System.IO.StreamReader(this.CreateTokenizedString("$(packagedir)/jmorecfg.h").Parse()))
                    {
                        return readFile.ReadToEnd();
                    }
                }
            }
        }
    }
}
