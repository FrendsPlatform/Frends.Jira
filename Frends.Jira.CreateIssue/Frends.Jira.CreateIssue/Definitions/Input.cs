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
    /// Project summary.
    /// </summary>
    /// <example>This is summary.</example>
    public string Summary { get; set; }

    /// <summary>
    /// Issue type.
    /// </summary>
    /// <example>Incident</example>
    public string IssueType { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    /// <example>This is description.</example>
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

/// <summary>
/// Procedure parameter.
/// </summary>
public class Parameters
{
    /// <summary>
    /// Key.
    /// </summary>
    /// <example>customfield_1</example>
    public string Key { get; set; }

    /// <summary>
    /// Value.
    /// </summary>
    /// <example>foo</example>
    public string Value { get; set; }
}