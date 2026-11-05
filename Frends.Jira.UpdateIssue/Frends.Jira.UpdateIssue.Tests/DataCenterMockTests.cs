using System.Net.Http.Headers;
using Frends.Jira.UpdateIssue.Definitions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace Frends.Jira.UpdateIssue.Tests;

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
            IdOrKey = "TT-1",
            Content = JObject.FromObject(new { fields = new { summary = "Updated summary" } }),
        };
        options = DefaultOptions();
    }

    [TearDown]
    public void Cleanup() => server.Stop();

    [Test]
    public async Task UsesV2AndBearerAuthenticationForUpdate()
    {
        StubUpdateIssue();

        var result = await Jira.UpdateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(server.LogEntries, Has.Count.EqualTo(1));
        Assert.That(
            SentAuthorization(0),
            Is.EqualTo(new AuthenticationHeaderValue("Bearer", Token)));

        Assert.That(
            JObject.Parse(server.LogEntries[0].RequestMessage!.Body!)[
                "fields"]!["summary"]!.ToString(),
            Is.EqualTo("Updated summary"));
    }

    [Test]
    public async Task IgnoresCloudApiVersion()
    {
        connection.CloudApiVersion = JiraCloudApiVersion.V3;
        StubUpdateIssue();

        var result = await Jira.UpdateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
        Assert.That(server.LogEntries, Has.Count.EqualTo(1));
        Assert.That(
            server.LogEntries[0].RequestMessage!.Path,
            Does.StartWith("/rest/api/2/"));
    }

    [Test]
    public async Task SupportsContextPath()
    {
        connection.JiraBaseUrl = $"{server.Url}/jira";
        server
            .Given(Request.Create().WithPath("/jira/rest/api/2/issue/TT-1").UsingPut())
            .RespondWith(Response.Create().WithStatusCode(204));

        var result = await Jira.UpdateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.True);
    }

    [Test]
    public async Task UpdateFailureIncludesDeploymentAndVersion()
    {
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingPut())
            .RespondWith(Response.Create().WithStatusCode(400).WithBody("{\"errorMessages\":[\"Invalid update\"]}"));
        options.ThrowErrorOnFailure = false;

        var result = await Jira.UpdateIssue(input, connection, options, default);

        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Does.Contain("Error updating issue (DataCenter, API v2): BadRequest"));
        Assert.That(result.Error.Message, Does.Contain("Invalid update"));
    }

    private void StubUpdateIssue() =>
        server
            .Given(Request.Create().WithPath("/rest/api/2/issue/TT-1").UsingPut())
            .RespondWith(Response.Create().WithStatusCode(204));

    private AuthenticationHeaderValue SentAuthorization(int index) =>
        AuthenticationHeaderValue.Parse(server.LogEntries[index].RequestMessage!.Headers!["Authorization"].First());
}
