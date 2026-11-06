using System.Net;
using Frends.Jira.DeleteIssue.Definitions;
using NUnit.Framework;

namespace Frends.Jira.DeleteIssue.Tests;

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

    [TestCase(JiraCloudApiVersion.V2)]
    [TestCase(JiraCloudApiVersion.V3)]
    public async Task DeleteIssue_ByKey_DeletesIssue(JiraCloudApiVersion version)
    {
        var connection = RequireCloudConnection(version);
        var issue = await CreateAndTrack(connection);
        var input = new Input { IdOrKey = issue.Key };

        var result = await Jira.DeleteIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True, result.Error?.Message);
        Assert.That(result.Error, Is.Null);
        Assert.That(await GetIssueStatus(connection, version, issue.Key), Is.EqualTo(HttpStatusCode.NotFound));
        created.Remove((connection, issue.Key));
    }

    [TestCase(JiraCloudApiVersion.V2)]
    [TestCase(JiraCloudApiVersion.V3)]
    public async Task DeleteIssue_ById_DeletesIssue(JiraCloudApiVersion version)
    {
        var connection = RequireCloudConnection(version);
        var issue = await CreateAndTrack(connection);
        var input = new Input { IdOrKey = issue.Id };

        var result = await Jira.DeleteIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True, result.Error?.Message);
        Assert.That(await GetIssueStatus(connection, version, issue.Key), Is.EqualTo(HttpStatusCode.NotFound));
        created.Remove((connection, issue.Key));
    }

    [Test]
    public async Task DeleteIssue_NonExistingKey_ReturnsError()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        options.ThrowErrorOnFailure = false;

        var result = await Jira.DeleteIssue(new Input { IdOrKey = "TT-99999999" }, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error retrieving issue (Cloud, API v3): NotFound"));
    }

    private async Task<(string Id, string Key)> CreateAndTrack(Connection connection)
    {
        var issue = await CreateIssue(connection);
        created.Add((connection, issue.Key));
        return issue;
    }
}
