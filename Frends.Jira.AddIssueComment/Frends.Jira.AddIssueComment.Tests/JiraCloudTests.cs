using Frends.Jira.AddIssueComment.Definitions;
using NUnit.Framework;

namespace Frends.Jira.AddIssueComment.Tests;

[TestFixture]
internal class JiraCloudTests : TestBase
{
    private const string Adf = """
        { "type": "doc", "version": 1, "content": [ { "type": "paragraph", "content": [ { "type": "text", "text": "ADF comment" } ] } ] }
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
                // Best effort cleanup; do not hide the test result.
            }
        }

        created.Clear();
    }

    [Test]
    public async Task AddIssueComment_V2_ByKey_AddsPlainTextComment()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V2);
        var issue = await CreateAndTrack(connection);
        var input = new Input { IdOrKey = issue.Key, Comment = "This is \"a\" comment." };

        var result = await Jira.AddIssueComment(input, connection, options, default);
        var comments = await GetComments(connection, issue.Key);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Error, Is.Null);
        Assert.That(comments, Has.Count.EqualTo(1));
        Assert.That(comments[0]["body"]!.ToString(), Does.Contain("This is \"a\" comment."));
    }

    [Test]
    public async Task AddIssueComment_V3_ById_AddsAdfComment()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        var issue = await CreateAndTrack(connection);
        var input = new Input { IdOrKey = issue.Id, Comment = Adf };

        var result = await Jira.AddIssueComment(input, connection, options, default);
        var comments = await GetComments(connection, issue.Key);

        Assert.That(result.Success, Is.True);
        Assert.That(comments, Has.Count.EqualTo(1));
        Assert.That(comments[0]["body"]!.ToString(), Does.Contain("ADF comment"));
    }

    [Test]
    public async Task AddIssueComment_V3_PlainText_ReturnsError()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        options.ThrowErrorOnFailure = false;
        var input = new Input { IdOrKey = "TT-1", Comment = "plain text" };

        var result = await Jira.AddIssueComment(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Atlassian Document Format"));
    }

    [Test]
    public async Task AddIssueComment_NonExistingKey_ReturnsError()
    {
        var connection = RequireCloudConnection(JiraCloudApiVersion.V3);
        options.ThrowErrorOnFailure = false;
        var input = new Input { IdOrKey = "TT-99999999", Comment = Adf };

        var result = await Jira.AddIssueComment(input, connection, options, default);

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
