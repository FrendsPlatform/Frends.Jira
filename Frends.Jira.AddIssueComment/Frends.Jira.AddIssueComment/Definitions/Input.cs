namespace Frends.Jira.AddIssueComment.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Issue ID or key.
    /// </summary>
    /// <example>TT-1, 123</example>
    public string IdOrKey { get; set; }

    /// <summary>
    /// Comment as string.
    /// </summary>
    /// <example>Comment here.</example>
    public string Comment { get; set; }
}