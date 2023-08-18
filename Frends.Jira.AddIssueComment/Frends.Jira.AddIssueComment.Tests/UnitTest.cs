using Frends.Jira.AddIssueComment.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;

namespace Frends.Jira.AddIssueComment.Tests;

[TestClass]
public class UnitTest
{
    private readonly string? _jiraUrl = Environment.GetEnvironmentVariable("TestJiraUrl");
    private readonly string? _pat = Environment.GetEnvironmentVariable("TestJiraPat");
    private Connection _connection = new();
    private Input _input = new();
    private Options _options = new();

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
            IdOrKey = "",
            Comment = ""
        };

        _options = new()
        {
            ThrowOnError = true,
        };
    }

    [TestMethod]
    public async Task UpdateIssue_Key_Success()
    {
        _input.IdOrKey = await CreateIssue(true);
        _input.Comment = "This is comment.";

        var result = await Jira.AddIssueComment(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);

        await DeleteIssue(_input.IdOrKey);
    }

    [TestMethod]
    public async Task UpdateIssue_Id_Success()
    {
        _input.IdOrKey = await CreateIssue(false);
        _input.Comment = "This is comment.";

        var result = await Jira.AddIssueComment(_connection, _input, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.ErrorMessage);

        await DeleteIssue(_input.IdOrKey);
    }

    [TestMethod]
    public async Task UpdateIssue_NotFound()
    {
        _input.IdOrKey = "Foo";
        _options.ThrowOnError = false;

        var result = await Jira.AddIssueComment(_connection, _input, _options, default);
        Assert.IsFalse(result.Success);
        Assert.IsTrue(result.ErrorMessage.Contains("Error retrieving issue, Status code: NotFound"));
    }

    [TestMethod]
    public async Task UpdateIssue_Exception()
    {
        var connection = _connection;
        connection.JiraBaseUrl = "";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => Jira.AddIssueComment(connection, _input, _options, default));
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
                summary = "This is for comment test",
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