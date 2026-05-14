#!/usr/bin/env dotnet run
#:property PublishAot=false
#:package Cake.Frosting.PleOps.Recipe@1.0.4-preview.62

using System.Diagnostics.CodeAnalysis;
using Cake.Common.IO;
using Cake.Common.Net;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Frosting.PleOps.Recipe;
using Cake.Frosting.PleOps.Recipe.Common;

return new CakeHost()
    .AddAssembly(typeof(PleOpsBuildContext).Assembly)
    .UseContext<PleOpsBuildContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public sealed class BuildLifetime : FrostingLifetime<PleOpsBuildContext>
{
    public override void Setup(PleOpsBuildContext context, ISetupContext info)
    {
        context.WarningsAsErrors = true;
        context.DotNetContext.CoverageTarget = 0;

        context.ReadArguments();

        context.DotNetContext.PreviewNuGetFeed = "https://pkgs.dev.azure.com/SceneGate/SceneGate/_packaging/SceneGate-Preview/nuget/v3/index.json";
        context.DotNetContext.AddApplication("./src/JUS.CLI", [ "win-x64", "linux-x64", "osx-x64" ]);

        context.ResourcesContext.ResourcesDirectory = Path.GetFullPath(Path.Combine("src", "JUS.Tests"));
        context.ResourcesContext.DownloadUser = "not_needed";
        context.ResourcesContext.DownloadFormat = ResourcesDownloadFormat.ZipBundle;
        context.ResourcesContext.DownloadId = "resources-0.zip";

        context.Print();
    }

    public override void Teardown(PleOpsBuildContext context, ITeardownContext info)
    {
        context.DeliveriesContext.Save();
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.CleanArtifactsTask))]
[IsDependentOn(typeof(DownloadJusTestFilesTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.BuildProjectTask))]
public sealed class DefaultTask : FrostingTask
{
}

[TaskName("Bundle")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHub.ExportReleaseNotesTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.BundleProjectTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.DocFx.BuildTask))]
public sealed class BundleTask : FrostingTask
{
}

[TaskName("Build-Bundle")]
[IsDependentOn(typeof(DefaultTask))]
[IsDependentOn(typeof(BundleTask))]
public sealed class BuildBundleTask : FrostingTask
{
}

[TaskName("Deploy")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.DeployProjectTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHub.UploadReleaseBinariesTask))]
public sealed class DeployTask : FrostingTask
{
}

[TaskName("Download-JusTestFiles")]
[TaskDescription("Download the test resource files")]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class DownloadJusTestFilesTask : FrostingTask<PleOpsBuildContext>
{
    public override bool ShouldRun(PleOpsBuildContext context) =>
        !string.IsNullOrEmpty(context.ResourcesContext.DownloadAddress);

    public override void Run(PleOpsBuildContext context)
    {
        ResourcesContext resourceInfo = context.ResourcesContext;
        string resourceUri = string.Format(resourceInfo.DownloadAddress!, resourceInfo.DownloadId);
        var downloadSettings = new DownloadFileSettings {
            Username = resourceInfo.DownloadUser,
            Password = resourceInfo.DownloadPassword,
            UseDefaultCredentials = string.IsNullOrWhiteSpace(resourceInfo.DownloadUser)
                || string.IsNullOrWhiteSpace(resourceInfo.DownloadPassword),
        };
        context.Log.Information(downloadSettings.UseDefaultCredentials
            ? "Download without credentials"
            : "Download will use provided password");

        context.Log.Information("Downloading resource");
        var compressedResources = context.DownloadFile(resourceUri, downloadSettings);

        context.Log.Debug("Unzipping resource");
        context.Unzip(compressedResources, context.ResourcesContext.ResourcesDirectory, true);
    }
}
