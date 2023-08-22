using Newtonsoft.Json.Linq;

namespace Frends.Jira.DeleteIssue.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>An error occured...</example>
    public string ErrorMessage { get; private set; }

    internal Result(bool success, string errorMessage)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }
}