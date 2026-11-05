using Frends.Jira.CreateIssue.Definitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace Frends.Jira.CreateIssue.Tests;

[TestFixture]
internal class DataCenterMockTests : TestBase
{
    private const string Token = "secret-token";

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

        connection = DefaultConnection();
        connection.JiraBaseUrl = server.Url!;
        connection.Token = Token;
        connection.DeploymentType = JiraDeploymentType.DataCenter;

        input = DefaultInput();
        input.IssueType = "Incident";

        options = DefaultOptions();
    }

    [TearDown]
    public void Cleanup() => server.Stop();

    [Test]
    public async Task UsesV2_BearerAndPlainDescription()
    {
        StubCreateIssue();

        var result = await Jira.CreateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(result.Data["key"]!.ToString(), Is.EqualTo("TT-1"));
        Assert.That(SentAuthHeader(), Is.EqualTo($"Bearer {Token}"));

        var fields = SentBody()["fields"]!;
        Assert.That(fields["description"]!.Type, Is.EqualTo(JTokenType.String));
        Assert.That(fields["description"]!.ToString(), Is.EqualTo("This is description"));
        Assert.That(fields["project"]!["key"]!.ToString(), Is.EqualTo("TT"));
        Assert.That(fields["issuetype"]!["name"]!.ToString(), Is.EqualTo("Incident"));
        Assert.That(fields["summary"]!.ToString(), Is.EqualTo("CreateIssue"));
    }

    [Test]
    public async Task IgnoresCloudApiVersion()
    {
        connection.CloudApiVersion = JiraCloudApiVersion.V3; // DC has only v2, so this must not matter
        StubCreateIssue();

        var result = await Jira.CreateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task WorksWithContextPath()
    {
        // Self-hosted Jira is often served under a context path, e.g. https://host/jira
        connection.JiraBaseUrl = $"{server.Url}/jira";
        StubCreateIssue(prefix: "/jira");

        var result = await Jira.CreateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task AdfLikeJsonInDescription_IsNotConverted()
    {
        // DC does not understand ADF, so everything goes through as a plain string.
        StubCreateIssue();
        input.Description = "{ \"type\": \"doc\", \"version\": 1, \"content\": [] }";

        await Jira.CreateIssue(input, connection, options, default);

        var description = SentBody()["fields"]!["description"]!;
        Assert.That(description.Type, Is.EqualTo(JTokenType.String));
        Assert.That(description.ToString(), Is.EqualTo(input.Description));
    }

    [Test]
    public async Task Parameters_StringsStayStrings_JsonIsSentAsJson()
    {
        StubCreateIssue();
        input.Parameters = new[]
        {
            new Parameters { Key = "customfield_11200", Value = "2023-08-15T13:10:00.000+0300" },
            new Parameters { Key = "labels", Value = "[\"a\", \"b\"]" },
            new Parameters { Key = "assignee", Value = "{ \"name\": \"jdoe\" }" }, // DC identifies users by name
            new Parameters { Key = "customfield_1", Value = "[not json" }, // broken JSON stays a string
        };

        await Jira.CreateIssue(input, connection, options, default);

        var fields = SentBody()["fields"]!;
        Assert.That(fields["customfield_11200"]!.Type, Is.EqualTo(JTokenType.String));
        Assert.That(fields["customfield_11200"]!.ToString(), Is.EqualTo("2023-08-15T13:10:00.000+0300"));
        Assert.That(fields["labels"]!.Type, Is.EqualTo(JTokenType.Array));
        Assert.That(fields["assignee"]!["name"]!.ToString(), Is.EqualTo("jdoe"));
        Assert.That(fields["customfield_1"]!.ToString(), Is.EqualTo("[not json"));
    }

    [Test]
    public async Task Unauthorized_ErrorContainsVariantAndStatus()
    {
        StubCreateIssue(401, new { message = "Unauthorized" });
        options.ThrowErrorOnFailure = false;

        var result = await Jira.CreateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error creating issue (DataCenter, API v2): Unauthorized"));
    }

    [Test]
    public async Task NonJsonErrorBody_DoesNotHideRealError()
    {
        // e.g. a 502 page from a proxy: JToken.Parse must not mask the real status.
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(502).WithBody("<html>Bad Gateway</html>"));
        options.ThrowErrorOnFailure = false;

        var result = await Jira.CreateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("BadGateway"));
    }

    [Test]
    public async Task Cloud_WithoutEmail_Throws()
    {
        var connection = DefaultConnection();
        connection.JiraBaseUrl = "https://example.atlassian.net";
        connection.Token = "x";
        connection.DeploymentType = JiraDeploymentType.Cloud;
        connection.Email = string.Empty;

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => Jira.CreateIssue(DefaultInput(), connection, DefaultOptions(), default));
        Assert.That(ex!.Message, Does.Contain("Email is required for Jira Cloud"));
    }

    private void StubCreateIssue(int status = 201, object? body = null, string prefix = "")
    {
        server
            .Given(Request.Create().WithPath($"{prefix}/rest/api/2/issue").UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(status)
                .WithHeader("Content-Type", "application/json")
                .WithBodyAsJson(body ?? new
                {
                    id = "1",
                    key = "TT-1",
                    self = $"{server.Url}{prefix}/rest/api/2/issue/1",
                }));
    }

    private JObject SentBody()
    {
        var body = server.LogEntries.Single().RequestMessage!.Body!;
        using var reader = new JsonTextReader(new StringReader(body))
        {
            DateParseHandling = DateParseHandling.None,
        };
        return JObject.Load(reader);
    }

    private string SentAuthHeader() =>
        server.LogEntries.Single().RequestMessage!.Headers!["Authorization"].First();
}
