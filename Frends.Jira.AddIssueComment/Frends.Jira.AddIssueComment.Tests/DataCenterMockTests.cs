using System.Net.Http.Headers;
using Frends.Jira.AddIssueComment.Definitions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace Frends.Jira.AddIssueComment.Tests;

[TestFixture]
internal class DataCenterMockTests : TestBase
{
    private const string Token = "test-token";

    private WireMockServer server = null!;
    private Input input = null!;
    private Connection connection = null!;
    private Options options = null!;

    [SetUp]
    public void Init()
    {
        server = WireMockServer.Start(new WireMockServerSettings
        {
            Urls = ["http://127.0.0.1:0"],
        });

        connection = new Connection
        {
            JiraBaseUrl = server.Url!,
            Token = Token,
            DeploymentType = JiraDeploymentType.DataCenter,
        };
        input = new Input { IdOrKey = "TT-1", Comment = "This is \"a\" comment." };
        options = DefaultOptions();
    }

    [TearDown]
    public void Cleanup() => server.Stop();

    [Test]
    public async Task UsesV2AndBearerAuthentication()
    {
        StubGetIssue();
        StubAddComment();

        var result = await Jira.AddIssueComment(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(server.LogEntries, Has.Count.EqualTo(2));
        Assert.That(SentAuthorization(0), Is.EqualTo(new AuthenticationHeaderValue("Bearer", Token)));
        Assert.That(SentAuthorization(1), Is.EqualTo(new AuthenticationHeaderValue("Bearer", Token)));
        Assert.That(
            JObject.Parse(server.LogEntries[1].RequestMessage!.Body!)["body"]!.ToString(),
            Is.EqualTo("This is \"a\" comment."));
    }

    [Test]
    public async Task IgnoresCloudApiVersion()
    {
        connection.CloudApiVersion = JiraCloudApiVersion.V3;
        StubGetIssue();
        StubAddComment();

        var result = await Jira.AddIssueComment(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(server.LogEntries, Has.Count.EqualTo(2));
        Assert.That(server.LogEntries[1].RequestMessage!.Path, Does.StartWith("/rest/api/2/"));
    }

    [Test]
    public async Task SupportsContextPath()
    {
        connection.JiraBaseUrl = $"{server.Url}/jira";
        server
            .Given(Request.Create().WithPath("/jira/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{}"));
        server
            .Given(Request.Create().WithPath("/jira/rest/api/2/issue/TT-1/comment").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(201).WithBody("{}"));

        var result = await Jira.AddIssueComment(input, connection, options, default);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task GetFailureIncludesDeploymentAndVersion()
    {
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(404).WithBody("{\"errorMessages\":[\"Issue does not exist\"]}"));
        options.ThrowErrorOnFailure = false;

        var result = await Jira.AddIssueComment(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error retrieving issue (DataCenter, API v2): NotFound"));
        Assert.That(result.Error.Message, Does.Contain("Issue does not exist"));
        Assert.That(server.LogEntries, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CommentFailureIncludesDeploymentAndVersion()
    {
        StubGetIssue();
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1/comment").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(403).WithBody("Forbidden by policy"));
        options.ThrowErrorOnFailure = false;

        var result = await Jira.AddIssueComment(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error adding comment (DataCenter, API v2): Forbidden"));
        Assert.That(result.Error.Message, Does.Contain("Forbidden by policy"));
    }

    [Test]
    public async Task MissingIdReturnsError()
    {
        input.IdOrKey = string.Empty;
        options.ThrowErrorOnFailure = false;

        var result = await Jira.AddIssueComment(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Issue ID or key is required"));
    }

    private void StubGetIssue() =>
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{}"));

    private void StubAddComment() =>
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1/comment").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(201).WithBody("{}"));

    private AuthenticationHeaderValue SentAuthorization(int index) =>
        AuthenticationHeaderValue.Parse(server.LogEntries[index].RequestMessage!.Headers!["Authorization"].First());
}
