string NuGetVersionV2 = "";
string SolutionFileName = "src/StoneAssemblies.Ecoflow.sln";

string[] DockerFiles = System.Array.Empty<string>();

string[] OutputImages = System.Array.Empty<string>();

string[] ComponentProjects  = new [] {
	"./src/StoneAssemblies.Ecoflow/StoneAssemblies.Ecoflow.csproj"
};


var ExecProjects  = new [] 
{
    "src/StoneAssemblies.Ecoflow.Cmd/StoneAssemblies.Ecoflow.Cmd.csproj"
};

var ExecProjectsOutputDirectories  = new [] 
{
    "output/release/ecoflow/{0}/{1}"
};

var RuntimeIdentifiers  = new string [] 
{
    "win-x86",
    "win-x64",
    "linux-x64",
    "linux-arm",
    "linux-arm64",
    "osx-x64"
};

var MauiProjects  = System.Array.Empty<string>();

var MauiProjectsOutputDirectories  = System.Array.Empty<string>();

var MauiFrameworkRuntimeIdentifiers  = new (string Framework, string RuntimeIdentifier) [] 
{
    (Framework:"net8.0-android", RuntimeIdentifier:string.Empty),
    // (Framework:"net6.0-windows10.0.19041.0", RuntimeIdentifier:"win10-x64"),
    // (Framework:"net6.0-windows10.0.19041.0", RuntimeIdentifier:"win10-x86"),
    // "net6.0-ios",
    // "net6.0-maccatalyst"
};

string TestProject = "src/StoneAssemblies.Ecoflow.Tests/StoneAssemblies.Ecoflow.Tests.csproj";

string SonarProjectKey = "stoneassemblies_StoneAssemblies.Ecoflow";
string SonarOrganization = "stoneassemblies";