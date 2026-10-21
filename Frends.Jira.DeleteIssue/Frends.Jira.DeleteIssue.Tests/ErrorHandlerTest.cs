using Frends.Jira.DeleteIssue.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Jira.DeleteIssue.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static readonly Input Input = new() { IdOrKey = "TT-1" };
    private static readonly Connection InvalidConnection = new() { JiraBaseUrl = null, Token = "token" };

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
            Jira.DeleteIssue(Input, InvalidConnection, new Options(), default));
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = new Options { ThrowErrorOnFailure = false };
        var result = await Jira.DeleteIssue(Input, InvalidConnection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = new Options { ErrorMessageOnFailure = CustomErrorMessage };
        var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
            Jira.DeleteIssue(Input, InvalidConnection, options, default));
        Assert.IsTrue(ex.Message.Contains(CustomErrorMessage));
    }
}
