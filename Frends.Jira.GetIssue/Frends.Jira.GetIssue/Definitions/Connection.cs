using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Jira.GetIssue.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Jira base URL.
    /// </summary>
    /// <example>https://test.jira.fi/jira</example>
    public string JiraBaseUrl { get; set; }

    /// <summary>
    /// PAT or API token.
    /// </summary>
    /// <example>1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string Token { get; set; }
}