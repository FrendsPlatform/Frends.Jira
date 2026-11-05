using Frends.Jira.UpdateIssue.Definitions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Frends.Jira.UpdateIssue.Tests;

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
    public async Task UpdateIssue_UpdatesFieldsUsingSelectedApiVersion(JiraCloudApiVersion version, string apiVersion)
    {
        var connection = RequireCloudConnection(version);
        var issue = await CreateAndTrack(connection);
        var updatedSummary = $"Updated by Frends test {Guid.NewGuid():N}";
        var input = new Input
        {
            IdOrKey = issue.Key,
            Content = JObject.FromObject(new { fields = new { summary = updatedSummary } }),
        };

        var result = await Jira.UpdateIssue(input, connection, options, default);
        var updatedIssue = await GetIssue(connection, version, issue.Key);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(updatedIssue["fields"]!["summary"]!.ToString(), Is.EqualTo(updatedSummary));
        Assert.That(updatedIssue["self"]!.ToString(), Does.Contain($"/rest/api/{apiVersion}/issue/"));
    }

    [Test]
    public async Task UpdateIssue_ById_UpdatesIssue()
    {
        var version = JiraCloudApiVersion.V3;
        var connection = RequireCloudConnection(version);
        var issue = await CreateAndTrack(connection);
        var updatedSummary = $"Updated by ID {Guid.NewGuid():N}";
        var input = new Input
        {
            IdOrKey = issue.Id,
            Content = JObject.FromObject(new { fields = new { summary = updatedSummary } }),
        };

        var result = await Jira.UpdateIssue(input, connection, options, default);
        var updatedIssue = await GetIssue(connection, version, issue.Key);

        Assert.That(result.Success, Is.True);
        Assert.That(updatedIssue["fields"]!["summary"]!.ToString(), Is.EqualTo(updatedSummary));
    }

    [Test]
    public async Task UpdateIssue_NonExistingKey_ReturnsError()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        options.ThrowErrorOnFailure = false;
        var input = new Input
        {
            IdOrKey = "TT-99999999",
            Content = JObject.FromObject(new { fields = new { summary = "Should not be applied" } }),
        };

        var result = await Jira.UpdateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error updating issue (Cloud, API v3): NotFound"));
    }

    private async Task<(string Id, string Key)> CreateAndTrack(Connection connection)
    {
        var issue = await CreateIssue(connection);
        created.Add((connection, issue.Key));
        return issue;
    }
}
