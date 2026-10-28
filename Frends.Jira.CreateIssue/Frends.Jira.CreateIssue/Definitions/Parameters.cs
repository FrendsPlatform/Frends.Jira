namespace Frends.Jira.CreateIssue.Definitions;

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
