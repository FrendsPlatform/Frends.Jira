using Frends.Jira.CreateIssue.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace Frends.Jira.CreateIssue.Tests;

[TestClass]
public class UnitTest
{
    private readonly string? _jiraUrl = Environment.GetEnvironmentVariable("TestJiraUrl");
    private readonly string? _pat = Environment.GetEnvironmentVariable("TestJiraPat");
    private Input _input = new();
    private Connection _connection = new();
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
            ProjectKey = "TT",
            Description = "This is description",
            IssueType = "Incident",
            Summary = $"CreateIssue",
            Parameters = new[]
            {
                new Parameters { Key = "customfield_11200", Value = "2023-08-15T13:10:00.000+0300" },
            }
        };

        _options = new()
        {
            ThrowErrorOnFailure = true,
        };
    }

    [TestMethod]
    public async Task CreateIssue_Success()
    {
        var result = await Jira.CreateIssue(_input, _connection, _options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNull(result.Error);
        await DeleteIssue(result.Data["key"]!.ToString());
    }

    [TestMethod]
    public async Task CreateIssue_NotFound()
    {
        _input.IssueType = "foo";
        _options.ThrowErrorOnFailure = false;

        var result = await Jira.CreateIssue(_input, _connection, _options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        StringAssert.Contains(result.Error.Message, "Error creating issue: BadRequest");
    }

    [TestMethod]
    public async Task CreateIssue_Exception()
    {
        _connection.JiraBaseUrl = "";

        var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() => Jira.CreateIssue(_input, _connection, _options, default));
        Assert.IsNotNull(ex);
        Assert.AreEqual("Parameter cannot be an empty string (Parameter 'baseUrl')", ex.Message);
    }

    [TestMethod]
    public async Task CreateIssue_Exception_ReturnsFailedResult()
    {
        _connection.JiraBaseUrl = "";
        _options.ThrowErrorOnFailure = false;

        var result = await Jira.CreateIssue(_input, _connection, _options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task CreateIssue_Exception_UsesCustomErrorMessage()
    {
        _connection.JiraBaseUrl = "";
        _options.ErrorMessageOnFailure = "CustomErrorMessage";

        var ex = await Assert.ThrowsExceptionAsync<Exception>(() => Jira.CreateIssue(_input, _connection, _options, default));
        StringAssert.Contains(ex.Message, "CustomErrorMessage");
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