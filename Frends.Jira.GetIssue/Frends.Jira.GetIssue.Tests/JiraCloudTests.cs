using Frends.Jira.GetIssue.Definitions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Jira.GetIssue.Tests;

[TestFixture]
internal class JiraCloudTests : TestBase
{
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
                // Best effort cleanup; do not hide the test result.
            }
        }

        created.Clear();
    }

    [TestCase(JiraCloudApiVersion.V2, "2")]
    [TestCase(JiraCloudApiVersion.V3, "3")]
    public async Task GetIssue_ByKey_UsesSelectedApiVersion(JiraCloudApiVersion version, string apiVersion)
    {
        var connection = RequireCloudConnection(version);
        var issue = await CreateAndTrack(connection);
        var result = await Jira.GetIssue(IdOrKeyInput(issue.Key), connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data["key"]!.ToString(), Is.EqualTo(issue.Key));
        Assert.That(result.Data["self"]!.ToString(), Does.Contain($"/rest/api/{apiVersion}/issue/"));
        Assert.That(result.Error, Is.Null);
    }

    [Test]
    public async Task GetIssue_ById_ReturnsIssue()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        var issue = await CreateAndTrack(connection);
        var result = await Jira.GetIssue(IdOrKeyInput(issue.Id), connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data["id"]!.ToString(), Is.EqualTo(issue.Id));
        Assert.That(result.Data["key"]!.ToString(), Is.EqualTo(issue.Key));
    }

    [TestCase(JiraCloudApiVersion.V2)]
    [TestCase(JiraCloudApiVersion.V3)]
    public async Task GetIssue_ByJql_FindsCreatedIssue(JiraCloudApiVersion version)
    {
        var connection = RequireCloudConnection(version);
        var issue = await CreateAndTrack(connection);
        var input = new Input
        {
            SearchType = SearchType.Jql,
            Jql = $"project = {Environment.GetEnvironmentVariable("TestJiraCloudProject") ?? "TT"} AND key = {issue.Key}",
        };

        // The search index is eventually consistent, so retry for a short time.
        JToken? match = null;
        for (var attempt = 0; attempt < 10 && match == null; attempt++)
        {
            var result = await Jira.GetIssue(input, connection, options, default);
            match = (result.Data["issues"] as JArray)?.FirstOrDefault(item => item["key"]?.ToString() == issue.Key);

            if (match == null)
                await Task.Delay(TimeSpan.FromSeconds(2));
        }

        Assert.That(match, Is.Not.Null, $"Issue {issue.Key} was not found by JQL after retries.");
        Assert.That(match!["fields"], Is.Not.Null, "Search result should include issue fields.");
    }

    [TestCase(JiraCloudApiVersion.V2, JTokenType.String)]
    [TestCase(JiraCloudApiVersion.V3, JTokenType.Object)]
    public async Task GetIssue_DescriptionFormat_DependsOnApiVersion(JiraCloudApiVersion version, JTokenType expectedType)
    {
        var connection = RequireCloudConnection(version);
        var issue = await CreateAndTrack(connection, "This is description");
        var result = await Jira.GetIssue(IdOrKeyInput(issue.Key), connection, options, default);

        Assert.That(result.Data["fields"]!["description"]!.Type, Is.EqualTo(expectedType));
    }

    [Test]
    public async Task GetIssue_NonExistingKey_ReturnsError()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        options.ThrowErrorOnFailure = false;

        var result = await Jira.GetIssue(IdOrKeyInput("TT-99999999"), connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error retrieving issue (Cloud, API v3): NotFound"));
    }

    [Test]
    public async Task GetIssue_InvalidJql_ReturnsError()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        options.ThrowErrorOnFailure = false;

        var input = new Input { SearchType = SearchType.Jql, Jql = "this is not valid jql ((" };
        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error retrieving issue (Cloud, API v3): BadRequest"));
    }

    private static Input IdOrKeyInput(string value) => new()
    {
        SearchType = SearchType.IdOrKey,
        IdOrKey = value,
    };

    private async Task<(string Id, string Key)> CreateAndTrack(Connection connection, string? description = null)
    {
        var issue = await CreateIssue(connection, description);
        created.Add((connection, issue.Key));
        return issue;
    }
}
