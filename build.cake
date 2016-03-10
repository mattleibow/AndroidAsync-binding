#tool nuget:?package=XamarinComponent

#addin "Cake.Xamarin"
#addin "Cake.XCode"
#addin "Cake.FileHelpers"

using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

////////////////////////////////////////////////////////////////////////////////////////////////////

var Target = Argument("t", Argument("target", Argument("Target", "Default")));
var Configuration = Argument("c", Argument("configuration", Argument("Configuration", "Release")));

var Version = "2.1.7";
var Url = "https://repo1.maven.org/maven2/com/koushikdutta/async/androidasync/" + Version + "/androidasync-" + Version + ".aar";
var AssemblyName = "AndroidAsync.dll";

////////////////////////////////////////////////////////////////////////////////////////////////////

DirectoryPath RootDir = MakeAbsolute(File(".")).GetDirectory();
DirectoryPath ExternalsDir = RootDir.Combine("externals");
DirectoryPath OutputDir = RootDir.Combine("output");
DirectoryPath ToolsDir = RootDir.Combine("tools");
DirectoryPath NuGetDir = RootDir.Combine("nuget");
DirectoryPath SourceDir = RootDir.Combine("AndroidAsync");

FilePath SolutionPath = RootDir.CombineWithFilePath("AndroidAsync.sln");
FilePath AarPath = ExternalsDir.CombineWithFilePath("androidasync.aar");
FilePath NuspecPath = NuGetDir.CombineWithFilePath("AndroidAsync.nuspec");

FilePath NuGetToolPath = ToolsDir.CombineWithFilePath("nuget.exe");
FilePath XamarinComponentToolPath = ToolsDir.CombineWithFilePath("XamarinComponent/tools/xamarin-component.exe");

////////////////////////////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(SourceDir.Combine("bin"));
    CleanDirectory(SourceDir.Combine("obj"));
    
    CleanDirectory(RootDir.Combine("AndroidAsyncSample/bin"));
    CleanDirectory(RootDir.Combine("AndroidAsyncSample/obj"));
    
    CleanDirectory(ExternalsDir);
    CleanDirectory(OutputDir);
    CleanDirectory(RootDir.Combine("packages"));
});

Task("Externals")
    .Does(() => 
{
    if (!DirectoryExists(ExternalsDir)) {
        CreateDirectory(ExternalsDir);
    }
    
    if (!FileExists(AarPath)) {
        DownloadFile(Url, AarPath);
    }
});

Task("Build")
    .IsDependentOn("Externals")
    .Does(() => 
{
    if (!DirectoryExists(OutputDir)) {
        CreateDirectory(OutputDir);
    }

    NuGetRestore(SolutionPath, new NuGetRestoreSettings { ToolPath = NuGetToolPath });
    
    if (IsRunningOnWindows()) {
        MSBuild(SolutionPath, s => s.SetConfiguration(Configuration).SetMSBuildPlatform(MSBuildPlatform.x86));
    } else {
        XBuild(SolutionPath, s => s.SetConfiguration(Configuration));
    }
    
    CopyFileToDirectory(SourceDir.Combine("bin").Combine(Configuration).CombineWithFilePath(AssemblyName), OutputDir);
});

Task("Package")
    .IsDependentOn("Build")
    .Does(() => 
{
    DeleteFiles(OutputDir.FullPath + "*.nupkg");
    
    NuGetPack(NuspecPath, new NuGetPackSettings { 
        Verbosity = NuGetVerbosity.Detailed,
        OutputDirectory = OutputDir.FullPath,		
        BasePath = IsRunningOnUnix() ? "././" : "./",
        ToolPath = NuGetToolPath
    });
});

////////////////////////////////////////////////////////////////////////////////////////////////////

Task("CI")
    .IsDependentOn("Build")
    .IsDependentOn("Package")
    .Does(() => 
{
});

RunTarget(Target);
