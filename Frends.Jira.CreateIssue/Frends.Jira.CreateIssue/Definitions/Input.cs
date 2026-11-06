namespace Frends.Jira.CreateIssue.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Project Key without number.
    /// </summary>
    /// <example>TT</example>
    public string ProjectKey { get; set; }

    /// <summary>
    /// Issue summary.
    /// </summary>
    /// <example>This is summary.</example>
    public string Summary { get; set; }

    /// <summary>
    /// Issue type.
    /// </summary>
    /// <example>Incident</example>
    public string IssueType { get; set; }

    /// <summary>
    /// Issue description. The format depends on the Jira version:
    /// Data Center and Cloud API v2 use plain text, for example: This is description.
    /// Cloud API v3 requires a valid Atlassian Document Format (ADF) JSON document. Plain text is not converted.
    /// </summary>
    /// <example>{ "type": "doc", "version": 1, "content": [ { "type": "paragraph", "content": [ { "type": "text", "text": "This is description." } ] } ] }</example>
    public string Description { get; set; }

    /// <summary>
    /// Optional parameters such as ticket creator (e.g. customfield_14102).
    /// </summary>
    /// <example>[
    /// { Key = "customfield_1", Value = "foo" },
    /// { Key = "customfield_2", Value = "bar" }
    /// ]</example>
    public Parameters[] Parameters { get; set; }
}
