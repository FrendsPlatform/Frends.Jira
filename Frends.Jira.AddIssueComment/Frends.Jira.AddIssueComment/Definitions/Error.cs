using System;

namespace Frends.Jira.AddIssueComment.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Error retrieving issue, Status code: NotFound</example>
    public string Message { get; set; }

    /// <summary>
    /// The original exception that caused the error.
    /// </summary>
    /// <example>System.Exception: Error retrieving issue</example>
    public Exception AdditionalInfo { get; set; }
}
