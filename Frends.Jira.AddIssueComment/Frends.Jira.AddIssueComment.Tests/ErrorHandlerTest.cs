using Frends.Jira.AddIssueComment.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Jira.AddIssueComment.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static readonly Input DefaultInput = new() { IdOrKey = "TT-1", Comment = "Comment" };
    private static readonly Connection InvalidConnection = new() { JiraBaseUrl = string.Empty, Token = "token" };

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
            Jira.AddIssueComment(DefaultInput, InvalidConnection, new Options(), default));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = await Jira.AddIssueComment(DefaultInput, InvalidConnection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
            Jira.AddIssueComment(DefaultInput, InvalidConnection, options, default));
        Assert.IsTrue(ex.Message.Contains(CustomErrorMessage));
    }
}
