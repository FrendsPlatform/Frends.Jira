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
    /// Request content as JSON string.
    /// </summary>
    /// <example>"{\"expand\":\"renderedFields,names,schema,operations,editmeta,changelog,versionedRepresentations\",\"id\":\"151894\",..."</example>
    public string Data { get; private set; }

    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>An error occured...</example>
    public string ErrorMessage { get; private set; }

    internal Result(bool success, string data, string errorMessage)
    {
        Success = success;
        Data = data;
        ErrorMessage = errorMessage;
    }
}