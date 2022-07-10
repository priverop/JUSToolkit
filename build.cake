// Remember to fix a version with "&version=x.y.z"
#load "nuget:?package=PleOps.Cake&prerelease"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("JUS.Tool");
    info.AddApplicationProjects("JUS.CLI");
    info.AddTestProjects("JUS.Tests");
    info.CoverageTarget = 0;

    info.PreviewNuGetFeed = "https://pkgs.dev.azure.com/SceneGate/SceneGate/_packaging/SceneGate-Preview/nuget/v3/index.json";

});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);
