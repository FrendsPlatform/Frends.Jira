using System;

namespace Frends.Jira.DeleteIssue.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Error retrieving issue: NotFound</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.Exception: Error retrieving issue: NotFound</example>
    public Exception AdditionalInfo { get; set; }
}
