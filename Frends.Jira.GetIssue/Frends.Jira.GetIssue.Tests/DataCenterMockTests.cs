using System.Net.Http.Headers;
using Frends.Jira.GetIssue.Definitions;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace Frends.Jira.GetIssue.Tests;

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
        input = new Input
        {
            SearchType = SearchType.IdOrKey,
            IdOrKey = "TT-1",
        };
        options = DefaultOptions();
    }

    [TearDown]
    public void Cleanup() => server.Stop();

    [Test]
    public async Task UsesV2AndBearerAuthentication()
    {
        StubGetIssue();

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data["key"]!.ToString(), Is.EqualTo("TT-1"));
        Assert.That(SentAuthorization(), Is.EqualTo(new AuthenticationHeaderValue("Bearer", Token)));
    }

    [Test]
    public async Task IgnoresCloudApiVersion()
    {
        connection.CloudApiVersion = JiraCloudApiVersion.V3;
        StubGetIssue();

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(server.LogEntries.Single().RequestMessage!.Path, Does.StartWith("/rest/api/2/"));
    }

    [Test]
    public async Task SupportsContextPath()
    {
        connection.JiraBaseUrl = $"{server.Url}/jira";
        server
            .Given(Request.Create().WithPath("/jira/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"key\":\"TT-1\"}"));

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task JqlSearchUsesEncodedQueryParameter()
    {
        input.SearchType = SearchType.Jql;
        input.Jql = "project = TT AND summary ~ \"a b\"";
        server
            .Given(Request.Create().WithPath("/rest/api/2/search").WithParam("jql", input.Jql).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"issues\":[]}"));

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data["issues"], Is.Not.Null);
    }

    [Test]
    public async Task NonJsonErrorBodyPreservesStatusAndBody()
    {
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(502).WithBody("<html>Bad Gateway</html>"));
        options.ThrowErrorOnFailure = false;

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error retrieving issue (DataCenter, API v2): BadGateway"));
        Assert.That(result.Error.Message, Does.Contain("Bad Gateway"));
    }

    private void StubGetIssue() =>
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"id\":\"1\",\"key\":\"TT-1\"}"));

    private AuthenticationHeaderValue SentAuthorization() =>
        AuthenticationHeaderValue.Parse(server.LogEntries.Single().RequestMessage!.Headers!["Authorization"].First());
}
