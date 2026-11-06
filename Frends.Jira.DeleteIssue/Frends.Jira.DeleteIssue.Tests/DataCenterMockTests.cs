using System.Net.Http.Headers;
using Frends.Jira.DeleteIssue.Definitions;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace Frends.Jira.DeleteIssue.Tests;

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
        input = new Input { IdOrKey = "TT-1" };
        options = DefaultOptions();
    }

    [TearDown]
    public void Cleanup() => server.Stop();

    [Test]
    public async Task UsesV2AndBearerAuthenticationForGetAndDelete()
    {
        StubGetIssue();
        StubDeleteIssue();

        var result = await Jira.DeleteIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(server.LogEntries, Has.Count.EqualTo(2));
        Assert.That(server.LogEntries[0].RequestMessage!.Path, Is.EqualTo("/rest/api/2/issue/TT-1"));
        Assert.That(server.LogEntries[1].RequestMessage!.Path, Is.EqualTo("/rest/api/2/issue/TT-1"));
        Assert.That(SentAuthorization(0), Is.EqualTo(new AuthenticationHeaderValue("Bearer", Token)));
        Assert.That(SentAuthorization(1), Is.EqualTo(new AuthenticationHeaderValue("Bearer", Token)));
    }

    [Test]
    public async Task IgnoresCloudApiVersion()
    {
        connection.CloudApiVersion = JiraCloudApiVersion.V3;
        StubGetIssue();
        StubDeleteIssue();

        var result = await Jira.DeleteIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(server.LogEntries.All(entry => entry.RequestMessage!.Path!.StartsWith("/rest/api/2/")), Is.True);
    }

    [Test]
    public async Task SupportsContextPath()
    {
        connection.JiraBaseUrl = $"{server.Url}/jira";
        server
            .Given(Request.Create().WithPath("/jira/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"key\":\"TT-1\"}"));
        server
            .Given(Request.Create().WithPath("/jira/rest/api/2/issue/TT-1").UsingDelete())
            .RespondWith(Response.Create().WithStatusCode(204));

        var result = await Jira.DeleteIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task GetFailureIncludesDeploymentVersionAndResponse()
    {
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(502).WithBody("<html>Bad Gateway</html>"));
        options.ThrowErrorOnFailure = false;

        var result = await Jira.DeleteIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error retrieving issue (DataCenter, API v2): BadGateway"));
        Assert.That(result.Error.Message, Does.Contain("Bad Gateway"));
        Assert.That(server.LogEntries, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task DeleteFailureIncludesDeploymentVersionAndResponse()
    {
        StubGetIssue();
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingDelete())
            .RespondWith(Response.Create().WithStatusCode(403).WithBody("{\"errorMessages\":[\"Forbidden\"]}"));
        options.ThrowErrorOnFailure = false;

        var result = await Jira.DeleteIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error deleting issue (DataCenter, API v2): Forbidden"));
    }

    [Test]
    public async Task MissingIssueKeyReturnsFailedResultWhenConfigured()
    {
        input.IdOrKey = " ";
        options.ThrowErrorOnFailure = false;

        var result = await Jira.DeleteIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Issue ID or key is required"));
        Assert.That(server.LogEntries, Is.Empty);
    }

    private void StubGetIssue() =>
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"id\":\"1\",\"key\":\"TT-1\"}"));

    private void StubDeleteIssue() =>
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingDelete())
            .RespondWith(Response.Create().WithStatusCode(204));

    private AuthenticationHeaderValue SentAuthorization(int index) =>
        AuthenticationHeaderValue.Parse(server.LogEntries[index].RequestMessage!.Headers!["Authorization"].First());
}
