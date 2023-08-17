namespace Frends.Jira.DeleteIssue.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Specify the issue key or ID you want to delete.
    /// </summary>
    /// <example>TT-1, 123</example>
    public string IdOrKey { get; set; }
}