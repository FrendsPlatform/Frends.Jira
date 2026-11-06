using dotenv.net;

using Frends.Jira.CreateIssue.Definitions;

namespace Frends.Jira.CreateIssue.Tests;

internal abstract class TestBase
{
    internal TestBase()
    {
        DotEnv.Load();
    }

    protected static Input DefaultInput() => new()
    {
        ProjectKey = "TT",
        Summary = "CreateIssue",
        IssueType = "Task",
        Description = "This is description",
    };

    protected static Connection DefaultConnection() => new();

    protected static Options DefaultOptions() => new() { ThrowErrorOnFailure = true };

    protected static Input CloudInput(string description)
    {
        var input = DefaultInput();
        input.ProjectKey = GetEnvVar("TestJiraCloudProject") ?? "TT";
        input.Summary = $"CreateIssue Cloud {DateTime.UtcNow:O}";
        input.Description = description;
        return input;
    }

    protected static Connection RequireCloudConnection(JiraCloudApiVersion version)
    {
        var url = GetEnvVar("TestJiraCloudUrl");
        var email = GetEnvVar("TestJiraCloudEmail");
        var token = GetEnvVar("TestJiraCloudToken");

        return new Connection
        {
            JiraBaseUrl = url!,
            Email = email!,
            Token = token!,
            DeploymentType = JiraDeploymentType.Cloud,
            CloudApiVersion = version,
        };
    }

    private static string GetEnvVar(string name) => Environment.GetEnvironmentVariable(name) ??
                                                       throw new InvalidOperationException(
                                                           $"Missing required env var: {name}");
}
