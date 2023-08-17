using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Jira.GetIssue.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Search type.
    /// </summary>
    /// <example>SearchType.IssueKey</example>
    [DefaultValue(SearchType.IssueKey)]
    public SearchType SearchType { get; set; }

    /// <summary>
    /// Specify the issue key you want to retrieve.
    /// </summary>
    /// <example>TT-1</example>
    [UIHint(nameof(SearchType), "", SearchType.IssueKey)]
    public string IssueKey { get; set; }

    /// <summary>
    /// Specify the issue ID you want to retrieve.
    /// </summary>
    /// <example>123</example>
    [UIHint(nameof(SearchType), "", SearchType.IssueId)]
    public string IssueId { get; set; }

    /// <summary>
    /// Use JQL to search for issues.
    /// </summary>
    /// <example>project=TT</example>
    [UIHint(nameof(SearchType), "", SearchType.Jql)]
    public string Jql { get; set; }
}