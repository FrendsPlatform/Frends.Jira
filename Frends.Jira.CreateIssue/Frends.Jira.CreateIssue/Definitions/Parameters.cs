namespace Frends.Jira.CreateIssue.Definitions;

/// <summary>
/// Optional issue field (e.g. a custom field).
/// </summary>
public class Parameters
{
    /// <summary>
    /// Jira field name or ID.
    /// </summary>
    /// <example>customfield_1</example>
    public string Key { get; set; }

    /// <summary>
    /// Field value. Plain text, or a JSON object/array for fields such as select, user or labels.
    /// User fields differ between Jira types: Cloud uses accountId, Data Center uses name.
    /// </summary>
    /// <example>foo</example>
    /// <example>{ "accountId": "5b10a2844c20165700ede21g" }</example>
    public string Value { get; set; }
}
