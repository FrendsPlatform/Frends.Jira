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
    [DefaultValue(SearchType.IdOrKey)]
    public SearchType SearchType { get; set; }

    /// <summary>
    /// Specify the issue ID or key to retrieve.
    /// </summary>
    /// <example>TT-1, 123</example>
    [UIHint(nameof(SearchType), "", SearchType.IdOrKey)]
    public string IdOrKey { get; set; }

    /// <summary>
    /// Use JQL to search for issues.
    /// </summary>
    /// <example>project=TT</example>
    [UIHint(nameof(SearchType), "", SearchType.Jql)]
    public string Jql { get; set; }
}