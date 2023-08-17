using Frends.Jira.GetIssue.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;

namespace Frends.Jira.GetIssue.Tests;

[TestClass]
public class UnitTest
{
    private readonly string? _jiraUrl = Environment.GetEnvironmentVariable("TestJiraUrl");
    private readonly string? _pat = Environment.GetEnvironmentVariable("TestJiraPat");
    private Input _input = new();
    private Connection _connection = new();

    [TestInitialize]
    public void Init()
    {
        _connection = new()
        {
            JiraBaseUrl = _jiraUrl,
            Token = _pat,
        };

        _input = new()
        {
            SearchType = SearchType.IdOrKey,
            IdOrKey = "",
            Jql = ""
        };
    }

    [TestMethod]
    public async Task GetIssue_Key_Success()
    {
        _input.IdOrKey = await CreateIssue(true);
        var result = await Jira.GetIssue(_connection, _input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);

        await DeleteIssue(_input.IdOrKey);
    }

    [TestMethod]
    public async Task GetIssue_Id_Success()
    {
        _input.IdOrKey = await CreateIssue(false);
        var result = await Jira.GetIssue(_connection, _input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);

        await DeleteIssue(_input.IdOrKey);
    }

    [TestMethod]
    public async Task GetIssue_All_Success()
    {
        _input.SearchType = SearchType.Jql;
        _input.IdOrKey = await CreateIssue(false);
        _input.Jql = "project=TT";

        var result = await Jira.GetIssue(_connection, _input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);

        await DeleteIssue(_input.IdOrKey);
    }

    [TestMethod]
    public async Task GetIssue_NotFound()
    {
        _input.IdOrKey = "Foo";

        var result = await Jira.GetIssue(_connection, _input, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.AreEqual("Error retrieving issue: NotFound", result.ErrorMessage);
    }

    [TestMethod]
    public async Task GetIssue_Exception()
    {
        _connection.JiraBaseUrl = "";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => Jira.GetIssue(_connection, _input, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("An error occurred: Value cannot be null. (Parameter 'baseUrl')", ex.Message);
    }

    private async Task<string> CreateIssue(bool key)
    {
        try
        {
            using var client = new RestClient(_connection.JiraBaseUrl);
            client.AddDefaultHeader("Authorization", $"Bearer {_connection.Token}");
            var request = new RestRequest("rest/api/latest/issue", Method.Post);

            var fields = new
            {
                summary = "This is for get test",
                issuetype = new { name = "Incident" },
                description = "Desc",
                project = new { key = "TT" }
            };

            request.AddJsonBody(new { fields });

            var response = await client.ExecuteAsync(request);
            var responseObject = JsonConvert.DeserializeAnonymousType(response.Content, new { key = "", id = "" });
            return key ? responseObject.key : responseObject.id;
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred: {ex.Message}");
        }
    }

    private async Task DeleteIssue(string value)
    {
        try
        {
            using var client = new RestClient(_connection.JiraBaseUrl);
            client.AddDefaultHeader("Authorization", $"Bearer {_connection.Token}");
            var request = new RestRequest($"rest/api/latest/issue/{value}", Method.Delete);
            await client.ExecuteAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred: {ex.Message}");
        }
    }
}