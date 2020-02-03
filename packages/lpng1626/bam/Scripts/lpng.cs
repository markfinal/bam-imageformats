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
namespace lpng
{
    [Bam.Core.ModuleGroup("Thirdparty/libpng")]
    class PNGLibrary :
        C.DynamicLibrary,
        C.IPublicHeaders
    {
        Bam.Core.TokenizedString C.IPublicHeaders.SourceRootDirectory { get; } = null;

        Bam.Core.StringArray C.IPublicHeaders.PublicHeaderPaths { get; } = new Bam.Core.StringArray(
            "png.h",
            "pngconf.h"
        );

        protected override void
        Init()
        {
            base.Init();

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                // to match that in the CMakeLists.txt
                this.Macros.Add(C.ModuleMacroNames.SharedObjectSONameFileExtension, this.CreateTokenizedString(".so.$(MajorVersion)$(MinorVersion)"));
                this.Macros.Add(C.ModuleMacroNames.DynamicLibraryFileExtension, this.CreateTokenizedString(".so.$(MajorVersion)$(MinorVersion).$(PatchVersion).0"));

                this.PrivatePatch(settings =>
                {
                    if (settings is C.ICommonLinkerSettingsLinux linuxLinker)
                    {
                        linuxLinker.SharedObjectName = this.CreateTokenizedString("$(dynamicprefix)$(OutputName)$(sonameext)");
                        linuxLinker.CanUseOrigin = true;
                        linuxLinker.RPath.AddUnique("$ORIGIN");

                        if (settings is C.ICommonLinkerSettings linker)
                        {
                            linker.Libraries.Add("-lm");
                        }
                    }
                });
            }

            this.SetSemanticVersion(1, 6, 26);
            this.Macros.FromName(Bam.Core.ModuleMacroNames.OutputName).Set("png$(MajorVersion)$(MinorVersion)", this);

            this.CreateHeaderCollection("$(packagedir)/*.h");

            var source = this.CreateCSourceCollection(
                "$(packagedir)/*.c",
                filter: new System.Text.RegularExpressions.Regex(@"^((?!.*example)(?!.*pngtest).*)$")
            );

            var generateConf = Graph.Instance.FindReferencedModule<GeneratePngConfHeader>();
            source.DependsOn(generateConf);
            source.UsePublicPatches(generateConf);

            // zlib dependency is not longer exposed in public headers
            source.CompileAgainstSDK<zlib.SDK>();
            this.LinkAgainstSDK<zlib.SDK>();

            source.PrivatePatch(settings =>
                {
                    if (settings is C.ICommonCompilerSettings compiler)
                    {
                        compiler.WarningsAsErrors = false;
                    }
                    if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                        if (this is C.IDynamicLibrary && settings is C.ICommonPreprocessorSettings preprocessor)
                        {
                            preprocessor.PreprocessorDefines.Add("PNG_BUILD_DLL");
                        }
                    }
                    if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;

                        if (this is C.IDynamicLibrary)
                        {
                            // enable brute force export
                            gccCompiler.Visibility = GccCommon.EVisibility.Default;
                        }
                    }
                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                        if (this is C.IDynamicLibrary)
                        {
                            // enable brute force export
                            clangCompiler.Visibility = ClangCommon.EVisibility.Default;
                        }
                    }
                    /*
                    if (this.BuildEnvironment.Configuration == EConfiguration.Debug)
                    {
                        preprocessor.PreprocessorDefines.Add("PNG_DEBUG");
                    }
                    */
                });
        }
    }
}
