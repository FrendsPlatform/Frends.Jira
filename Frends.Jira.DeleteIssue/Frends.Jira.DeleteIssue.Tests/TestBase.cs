using System.Text;
using dotenv.net;
using Frends.Jira.DeleteIssue.Definitions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Frends.Jira.DeleteIssue.Tests;

internal abstract class TestBase
{
    protected TestBase()
    {
        DotEnv.Load();
    }

    protected static Options DefaultOptions() => new() { ThrowErrorOnFailure = true };

    protected static Connection RequireCloudConnection(JiraCloudApiVersion version) => new()
    {
        JiraBaseUrl = GetEnvVar("TestJiraCloudUrl"),
        Email = GetEnvVar("TestJiraCloudEmail"),
        Token = GetEnvVar("TestJiraCloudToken"),
        DeploymentType = JiraDeploymentType.Cloud,
        CloudApiVersion = version,
    };

    protected static async Task<(string Id, string Key)> CreateIssue(Connection connection)
    {
        using var client = CreateCloudClient(connection);
        var request = new RestRequest("rest/api/2/issue", Method.Post);
        request.AddJsonBody(new
        {
            fields = new
            {
                summary = $"DeleteIssue test {Guid.NewGuid():N}",
                issuetype = new { name = "Task" },
                project = new { key = Environment.GetEnvironmentVariable("TestJiraCloudProject") ?? "TT" },
            },
        });

        var response = await client.ExecuteAsync(request);
        if (!response.IsSuccessful)
        {
            throw new Exception($"Could not create test issue: {response.StatusCode} {response.Content}");
        }

        var issue = JObject.Parse(response.Content!);
        return (issue["id"]!.ToString(), issue["key"]!.ToString());
    }

    protected static async Task DeleteIssue(Connection connection, string key)
    {
        using var client = CreateCloudClient(connection);
        var response = await client.ExecuteAsync(new RestRequest($"rest/api/2/issue/{key}", Method.Delete));
        if (!response.IsSuccessful)
        {
            throw new Exception($"Could not clean up test issue {key}: {response.StatusCode} {response.Content}");
        }
    }

    protected static async Task<System.Net.HttpStatusCode> GetIssueStatus(Connection connection, JiraCloudApiVersion version, string key)
    {
        using var client = CreateCloudClient(connection);
        var apiVersion = version == JiraCloudApiVersion.V3 ? "3" : "2";
        var response = await client.ExecuteAsync(new RestRequest($"rest/api/{apiVersion}/issue/{key}", Method.Get));
        return response.StatusCode;
    }

    private static RestClient CreateCloudClient(Connection connection)
    {
        var client = new RestClient(connection.JiraBaseUrl);
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{connection.Email}:{connection.Token}"));
        client.AddDefaultHeader("Authorization", $"Basic {credentials}");
        return client;
    }

    private static string GetEnvVar(string name) => Environment.GetEnvironmentVariable(name) ??
        throw new InvalidOperationException($"Missing required env var: {name}");
}
