namespace Frends.Jira.GetIssue.Definitions;

/// <summary>
/// Search types.
/// </summary>
public enum SearchType
{
    /// <summary>
    /// Specify the issue key you want to retrieve.
    /// </summary>
    IssueKey,

    /// <summary>
    /// Specify the issue ID you want to retrieve.
    /// </summary>
    IssueId,

    /// <summary>
    /// Use JQL to search for issues.
    /// </summary>
    Jql
}