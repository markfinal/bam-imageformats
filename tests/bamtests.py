from testconfigurations import TestSDKSetup, visualc, visualc64, visualc32, mingw32, gcc, gcc64, gcc32, clang, clang32, clang64

def configure_repository():
    configs = {}
    configs["jpegtest1"] = TestSDKSetup(win={"Native": [visualc64, visualc32, mingw32], "VSSolution": [visualc64, visualc32], "MakeFile": [visualc64, visualc32, mingw32]},
                                        linux={"Native": [gcc64, gcc32], "MakeFile": [gcc64, gcc32]},
                                        osx={"Native": [clang64, clang32], "MakeFile": [clang64, clang32], "Xcode": [clang64, clang32]})
    configs.setdefault("pngtest1", [])
    configs["pngtest1"].append(TestSDKSetup(win={"Native": [visualc64, visualc32, mingw32], "VSSolution": [visualc64, visualc32], "MakeFile": [visualc64, visualc32, mingw32]},
                                            linux={"Native": [gcc64, gcc32], "MakeFile": [gcc64, gcc32]},
                                            osx={"Native": [clang64, clang32], "MakeFile": [clang64, clang32], "Xcode": [clang64, clang32]},
                                            options="--lpng.version=1257",
                                            alias="png1.2.x"))
    configs["pngtest1"].append(TestSDKSetup(win={"Native": [visualc64, visualc32, mingw32], "VSSolution": [visualc64, visualc32], "MakeFile": [visualc64, visualc32, mingw32]},
                                            linux={"Native": [gcc64, gcc32], "MakeFile": [gcc64, gcc32]},
                                            osx={"Native": [clang64, clang32], "MakeFile": [clang64, clang32], "Xcode": [clang64, clang32]},
                                            options="--lpng.version=1626",
                                            alias="png1.6.x"))
    configs["tifftest1"] = TestSDKSetup(win={"Native": [visualc64, visualc32, mingw32], "VSSolution": [visualc64, visualc32], "MakeFile": [visualc64, visualc32, mingw32]},
                                        linux={"Native": [gcc64, gcc32], "MakeFile": [gcc64, gcc32]},
                                        osx={"Native": [clang64, clang32], "MakeFile": [clang64, clang32], "Xcode": [clang64, clang32]})
    return configs
