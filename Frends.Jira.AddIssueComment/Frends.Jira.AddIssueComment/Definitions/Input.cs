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
    /// Comment text. Jira Cloud API v3 requires a valid Atlassian Document Format (ADF) JSON document. Plain text is not converted.
    /// Use API v2 or Data Center for plain text.
    /// </summary>
    /// <example>Comment here.</example>
    public string Comment { get; set; }
}
