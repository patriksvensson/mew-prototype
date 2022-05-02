var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

////////////////////////////////////////////////////////////////
// Tasks

Task("Build")
    .Does(context => 
{
    DotNetBuild("./src/Mew.sln", new DotNetBuildSettings {
        Configuration = configuration,
        NoIncremental = context.HasArgument("rebuild"),
        MSBuildSettings = new DotNetMSBuildSettings()
            .TreatAllWarningsAs(MSBuildTreatAllWarningsAs.Error)
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(context => 
{
    DotNetTest("./src/Mew.sln", new DotNetTestSettings {
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
    });
});

////////////////////////////////////////////////////////////////
// Targets

Task("Default")
    .IsDependentOn("Test");

////////////////////////////////////////////////////////////////
// Execution

RunTarget(target)