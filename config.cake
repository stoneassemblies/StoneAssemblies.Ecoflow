string NuGetVersionV2 = "";
string SolutionFileName = "src/StoneAssemblies.Ecoflow.sln";

string[] DockerFiles = System.Array.Empty<string>();

string[] OutputImages = System.Array.Empty<string>();

string[] ComponentProjects  = new [] {
	"./src/StoneAssemblies.Ecoflow/StoneAssemblies.Ecoflow.csproj"
};

string TestProject = "src/StoneAssemblies.Ecoflow.Tests/StoneAssemblies.Ecoflow.Tests.csproj";

string SonarProjectKey = "stoneassemblies_StoneAssemblies.Ecoflow";
string SonarOrganization = "stoneassemblies";