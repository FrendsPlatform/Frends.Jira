using System.Net.Http.Headers;
using Frends.Jira.GetIssue.Definitions;
using Newtonsoft.Json.Linq;
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
        input = JqlInput();
        input.Jql = "project = TT AND summary ~ \"a b\"";
        server
            .Given(Request.Create().WithPath("/rest/api/2/search").WithParam("jql", input.Jql).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"issues\":[]}"));

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data["issues"], Is.Not.Null);
    }

    [Test]
    public async Task JqlSearchCombinesAllDataCenterPages()
    {
        input = JqlInput();
        input.GetAllPages = true;
        input.MaxResults = 2;
        StubSearchPage(0, 2, "{\"startAt\":0,\"total\":3,\"issues\":[{\"key\":\"TT-1\"},{\"key\":\"TT-2\"}]}");
        StubSearchPage(2, 2, "{\"startAt\":2,\"total\":3,\"issues\":[{\"key\":\"TT-3\"}]}");

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(((JArray)result.Data["issues"]!).Select(issue => issue["key"]!.ToString()), Is.EqualTo(new[] { "TT-1", "TT-2", "TT-3" }));
        Assert.That(server.LogEntries, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task JqlSearchStopsAtMaxIssues()
    {
        input = JqlInput();
        input.GetAllPages = true;
        input.MaxResults = 2;
        input.MaxIssues = 3;
        StubSearchPage(0, 2, "{\"startAt\":0,\"total\":5,\"issues\":[{\"key\":\"TT-1\"},{\"key\":\"TT-2\"}]}");
        StubSearchPage(2, 1, "{\"startAt\":2,\"total\":5,\"issues\":[{\"key\":\"TT-3\"},{\"key\":\"TT-4\"}]}");

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(((JArray)result.Data["issues"]!).Count, Is.EqualTo(3));
        Assert.That(server.LogEntries, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task JqlSearchReturnsOnlyFirstPageByDefault()
    {
        input = JqlInput();
        input.MaxResults = 2;
        StubSearchPage(0, 2, "{\"startAt\":0,\"total\":5,\"issues\":[{\"key\":\"TT-1\"},{\"key\":\"TT-2\"}]}");

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(((JArray)result.Data["issues"]!).Count, Is.EqualTo(2));
        Assert.That(server.LogEntries, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task JqlSearchCanFetchDataCenterPageAtStartAt()
    {
        input = JqlInput();
        input.StartAt = 2;
        StubSearchPage(2, 100, "{\"startAt\":2,\"total\":4,\"issues\":[{\"key\":\"TT-3\"},{\"key\":\"TT-4\"}]}");

        var result = await Jira.GetIssue(input, connection, options, default);

        Assert.That(((JArray)result.Data["issues"]!).Select(issue => issue["key"]!.ToString()), Is.EqualTo(new[] { "TT-3", "TT-4" }));
        Assert.That(server.LogEntries.Single().RequestMessage!.Query!.Single(parameter => parameter.Key == "startAt").Value.Single(), Is.EqualTo("2"));
    }

    [Test]
    public async Task InvalidPaginationInputIsRejectedWithoutNetworkRequest()
    {
        input = JqlInput();
        input.MaxResults = 0;

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await Jira.GetIssue(input, connection, options, default));
        Assert.That(server.LogEntries, Is.Empty);
    }

    [TestCase("next-token", null, false, "NextPageToken is only supported for Jira Cloud.")]
    [TestCase(null, 2, true, "StartAt cannot be used when GetAllPages is true.")]
    [TestCase(null, -1, false, "StartAt must be zero or greater.")]
    public async Task InvalidDataCenterPagingOptionsAreRejectedWithoutNetworkRequest(string nextPageToken, int? startAt, bool getAllPages, string message)
    {
        input = JqlInput();
        input.NextPageToken = nextPageToken;
        input.StartAt = startAt;
        input.GetAllPages = getAllPages;

        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await Jira.GetIssue(input, connection, options, default));

        Assert.That(exception!.Message, Does.Contain(message));
        Assert.That(server.LogEntries, Is.Empty);
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

    private static Input JqlInput() => new()
    {
        SearchType = SearchType.Jql,
        Jql = "project = TT",
    };

    private void StubSearchPage(int startAt, int maxResults, string body) =>
        server
            .Given(Request.Create()
                .WithPath("/rest/api/2/search")
                .WithParam("startAt", startAt.ToString())
                .WithParam("maxResults", maxResults.ToString())
                .UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody(body));

    private void StubGetIssue() =>
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{\"id\":\"1\",\"key\":\"TT-1\"}"));

    private AuthenticationHeaderValue SentAuthorization() =>
        AuthenticationHeaderValue.Parse(server.LogEntries.Single().RequestMessage!.Headers!["Authorization"].First());
}
