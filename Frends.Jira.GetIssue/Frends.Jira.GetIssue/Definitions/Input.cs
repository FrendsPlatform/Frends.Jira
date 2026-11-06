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
    /// Use JQL to search for issues. To fetch a later page manually, provide the returned Cloud nextPageToken or the Data Center startAt offset below.
    /// </summary>
    /// <example>project=TT</example>
    [UIHint(nameof(SearchType), "", SearchType.Jql)]
    public string Jql { get; set; }

    /// <summary>
    /// Cloud cursor from the previous response's nextPageToken; set it to fetch the next page. Cannot be used with GetAllPages.
    /// </summary>
    /// <example>eyJvZmZzZXQiOjEwMH0=</example>
    [UIHint(nameof(SearchType), "", SearchType.Jql)]
    public string NextPageToken { get; set; }

    /// <summary>
    /// Data Center offset for the page to fetch; use the previous response's startAt plus its issue count. Cannot be used with GetAllPages.
    /// </summary>
    /// <example>50</example>
    [UIHint(nameof(SearchType), "", SearchType.Jql)]
    public int? StartAt { get; set; }

    /// <summary>
    /// Fetch and combine every page. By default, only the first page is returned.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(false)]
    [UIHint(nameof(SearchType), "", SearchType.Jql)]
    public bool GetAllPages { get; set; }

    /// <summary>
    /// Maximum number of issues Jira returns per page.
    /// </summary>
    /// <example>50</example>
    [DefaultValue(100)]
    [Range(1, int.MaxValue)]
    [UIHint(nameof(SearchType), "", SearchType.Jql)]
    public int MaxResults { get; set; } = 100;

    /// <summary>
    /// Maximum number of issues combined when GetAllPages is true.
    /// </summary>
    /// <example>1000</example>
    [DefaultValue(1000)]
    [Range(1, int.MaxValue)]
    [UIHint(nameof(GetAllPages), "", true)]
    public int MaxIssues { get; set; } = 1000;
}
