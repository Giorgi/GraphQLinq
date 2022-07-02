using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.CoverallsNet;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.CoverallsNet.CoverallsNetTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
[AppVeyor(AppVeyorImage.VisualStudio2022, InvokedTargets = new[] { nameof(Test) }, Secrets = new[] { "COVERALLS_REPO_TOKEN:0wXxQdMQF5yHgpUe3heAG5zY2YYNBZYEF9FmgqoQi6b6IC0JibAA+idcnOkwTeEg" })]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    
    readonly string[] ProjectsToPack = { "Client", "Scaffolding" };

    AbsolutePath SourceDirectory => RootDirectory;
    AbsolutePath TestDirectory => SourceDirectory / "GraphQLinq.Tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath TestResultsDirectory => SourceDirectory / "TestResults";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            EnsureCleanDirectory(TestResultsDirectory);

            DotNetTest(s => s
                .SetProjectFile(TestDirectory / "Tests.csproj")
                .SetResultsDirectory(TestResultsDirectory)
                .AddLoggers("trx")
                .EnableCollectCoverage()
                .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                .SetCoverletOutput("../TestResults/")
                .EnableNoRestore());

            CoverallsNet(s => s.EnableOpenCover().SetInput(TestResultsDirectory / "coverage.opencover.xml"));
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            var projects = from project in ProjectsToPack
                           select Solution.GetProject(project);

            DotNetPack(s => s
                .SetConfiguration("Release")
                .SetOutputDirectory(OutputDirectory)
                .SetProperty("SolutionName", Solution.Name)
                .CombineWith(projects, (settings, project) => settings.SetProject(project)));
        });
}
