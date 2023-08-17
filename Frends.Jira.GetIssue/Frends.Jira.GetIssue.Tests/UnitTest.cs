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
            SearchType = SearchType.IssueKey,
            IssueId = "",
            IssueKey = "",
            Jql = "",
        };
    }

    [TestMethod]
    public async Task GetIssue_Key_Success()
    {
        _input.IssueKey = await CreateIssue(true);
        var result = await Jira.GetIssue(_connection, _input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);

        await DeleteIssue(null, _input.IssueKey);
    }

    [TestMethod]
    public async Task GetIssue_Id_Success()
    {
        _input.SearchType = SearchType.IssueId;
        _input.IssueId = await CreateIssue(false);
        var result = await Jira.GetIssue(_connection, _input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);

        await DeleteIssue(_input.IssueId, null);
    }

    [TestMethod]
    public async Task GetIssue_All_Success()
    {
        _input.SearchType = SearchType.Jql;
        _input.IssueId = await CreateIssue(false);
        _input.Jql = "project=TT";

        var result = await Jira.GetIssue(_connection, _input, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.ErrorMessage);

        await DeleteIssue(_input.IssueId, null);
    }

    [TestMethod]
    public async Task GetIssue_NotFound()
    {
        _input.IssueKey = "Foo";

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
            var request = new RestRequest("rest/api/latest/issue", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_connection.Token}");

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

    private async Task DeleteIssue(string? id, string? key)
    {
        try
        {
            using var client = new RestClient(_connection.JiraBaseUrl);
            RestRequest request = new();

            if (!string.IsNullOrWhiteSpace(id))
                request = new RestRequest($"rest/api/latest/issue/{id}", Method.Delete);
            else if (!string.IsNullOrWhiteSpace(key))
                request = new RestRequest($"rest/api/latest/issue/{key}", Method.Delete);

            var deleteResponse = await client.ExecuteAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred: {ex.Message}");
        }
    }
}