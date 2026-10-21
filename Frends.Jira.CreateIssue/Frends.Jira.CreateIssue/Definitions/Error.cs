using System;

namespace Frends.Jira.CreateIssue.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Error creating issue: BadRequest</example>
    public string Message { get; set; }

    /// <summary>
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.Exception: Error creating issue: BadRequest</example>
    public Exception AdditionalInfo { get; set; }
}
