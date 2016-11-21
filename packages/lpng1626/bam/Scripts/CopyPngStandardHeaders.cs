using Bam.Core;
namespace lpng
{
    [ModuleGroup("Thirdparty/libpng")]
    class CopyPngStandardHeaders :
        Publisher.Collation
    {
        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            // the build mode depends on whether this path has been set or not
            if (this.GeneratedPaths.ContainsKey(Key))
            {
                this.GeneratedPaths[Key].Aliased(this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders"));
            }
            else
            {
                this.RegisterGeneratedFile(Key, this.CreateTokenizedString("$(packagebuilddir)/PublicHeaders"));
            }

            this.PublicPatch((settings, appliedTo) =>
            {
                var compiler = settings as C.ICommonCompilerSettings;
                if (null != compiler)
                {
                    compiler.IncludePaths.AddUnique(this.GeneratedPaths[Key]);
                }
            });

            var pngHeader = this.IncludeFile(this.CreateTokenizedString("$(packagedir)/png.h"), ".");
            this.IncludeFile(this.CreateTokenizedString("$(packagedir)/pngconf.h"), ".", pngHeader);
        }
    }
}
