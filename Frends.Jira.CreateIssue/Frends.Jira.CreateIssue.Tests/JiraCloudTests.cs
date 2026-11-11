using System.Text;
using Frends.Jira.CreateIssue.Definitions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;

namespace Frends.Jira.CreateIssue.Tests;

[TestFixture]
internal class JiraCloudTests : TestBase
{
    private const string SimpleAdf = """
    {
      "type": "doc",
      "version": 1,
      "content": [
        { "type": "paragraph", "content": [ { "type": "text", "text": "This is description" } ] }
      ]
    }
    """;

    private readonly List<(Connection Connection, string Key)> created = new();
    private Options options = null!;

    [SetUp]
    public void Init() => options = DefaultOptions();

    [TearDown]
    public async Task Cleanup()
    {
        foreach (var (connection, key) in created)
        {
            try
            {
                await DeleteIssue(connection, key);
            }
            catch
            {
                /* best effort, do not hide the real test failure */
            }
        }

        created.Clear();
    }

    [TestCase(JiraCloudApiVersion.V2, "2")]
    [TestCase(JiraCloudApiVersion.V3, "3")]
    public async Task Description_CreatesIssue(JiraCloudApiVersion version, string path)
    {
        var connection = RequireCloudConnection(version);

        var description = version == JiraCloudApiVersion.V3 ? SimpleAdf : "This is description";

        var (result, _) = await Create(CloudInput(description), connection);

        Assert.That(result.Error, Is.Null);
        Assert.That(result.Data["self"]!.ToString(), Does.Contain($"/rest/api/{path}/issue/"));
    }

    [Test]
    public async Task V3_ReadyAdf_IsStoredWithFormatting()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);

        const string adf = """
        {
          "type": "doc",
          "version": 1,
          "content": [
            { "type": "heading", "attrs": { "level": 2 }, "content": [ { "type": "text", "text": "Title" } ] },
            { "type": "paragraph", "content": [ { "type": "text", "text": "Bold text", "marks": [ { "type": "strong" } ] } ] }
          ]
        }
        """;

        var (_, key) = await Create(CloudInput(adf), connection);

        var issue = await GetIssue(connection, key);
        var content = (JArray)issue["fields"]!["description"]!["content"]!;
        Assert.That(content[0]["type"]!.ToString(), Is.EqualTo("heading"));
        Assert.That(content[0]["content"]![0]!["text"]!.ToString(), Is.EqualTo("Title"));
        Assert.That(content[1]["content"]![0]!["marks"]![0]!["type"]!.ToString(), Is.EqualTo("strong"));
    }

    [Test]
    public async Task V3_LabelsAsJsonArray()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);

        var input = CloudInput(SimpleAdf);
        input.Parameters = new[] { new Parameters { Key = "labels", Value = "[\"frends-test\", \"automated\"]" } };

        var (_, key) = await Create(input, connection);

        var issue = await GetIssue(connection, key);
        var labels = ((JArray)issue["fields"]!["labels"]!).Select(l => l.ToString()).ToList();
        Assert.That(labels, Is.EquivalentTo(new[] { "frends-test", "automated" }));
    }

    [Test]
    public async Task InvalidIssueType_Fails()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);

        var input = CloudInput(SimpleAdf);
        input.IssueType = "foo";
        options.ThrowErrorOnFailure = false;

        var result = await Jira.CreateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error creating issue (Cloud, API v3): BadRequest"));
    }

    [Test]
    public async Task V3_PlainText_IsRejectedByJira()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        options.ThrowErrorOnFailure = false;

        var result = await Jira.CreateIssue(CloudInput("plain text"), connection, options, default);

        if (result.Success) created.Add((connection, result.Data["key"]!.ToString()));
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error creating issue (Cloud, API v3): BadRequest"));
    }

    [TestCase(JiraCloudApiVersion.V2)]
    [TestCase(JiraCloudApiVersion.V3)]
    public async Task EmptyDescription_CreatesIssue(JiraCloudApiVersion version)
    {
        var connection = RequireCloudConnection(version);

        var (result, _) = await Create(CloudInput(string.Empty), connection);

        Assert.That(result.Error, Is.Null);
    }

    private static RestClient CreateClient(Connection connection)
    {
        var client = new RestClient(connection.JiraBaseUrl);
        var raw = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{connection.Email}:{connection.Token}"));
        client.AddDefaultHeader("Authorization", $"Basic {raw}");
        return client;
    }

    private static async Task<JObject> GetIssue(Connection connection, string key)
    {
        using var client = CreateClient(connection);
        var response = await client.ExecuteAsync(new RestRequest($"rest/api/3/issue/{key}", Method.Get));
        Assert.That(response.IsSuccessful, Is.True, $"Could not read issue {key}: {response.StatusCode} {response.Content}");
        return JObject.Parse(response.Content!);
    }

    private static async Task DeleteIssue(Connection connection, string key)
    {
        using var client = CreateClient(connection);
        var response = await client.ExecuteAsync(new RestRequest($"rest/api/2/issue/{key}", Method.Delete));
        if (!response.IsSuccessful)
            throw new Exception($"Could not delete issue {key}: {response.StatusCode} {response.Content}");
    }

    private async Task<(Result Result, string Key)> Create(Input input, Connection connection)
    {
        var result = await Jira.CreateIssue(input, connection, options, default);
        Assert.That(result.Success, Is.True);
        var key = result.Data["key"]!.ToString();
        created.Add((connection, key));
        return (result, key);
    }
}
