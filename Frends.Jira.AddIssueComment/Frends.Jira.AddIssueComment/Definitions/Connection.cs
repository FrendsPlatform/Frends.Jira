using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Jira.AddIssueComment.Definitions;

/// <summary>
/// Connection parameters.
/// </summary>
public class Connection
{
    /// <summary>
    /// Jira base URL. For Data Center include the context path if one is used.
    /// </summary>
    /// <example>https://yourcompany.atlassian.net</example>
    public string JiraBaseUrl { get; set; }

    /// <summary>
    /// Jira deployment type. Cloud uses Basic authentication (email and API token).
    /// Data Center uses REST API v2 and a Personal Access Token (PAT).
    /// </summary>
    /// <example>JiraDeploymentType.Cloud</example>
    [DefaultValue(JiraDeploymentType.Cloud)]
    public JiraDeploymentType DeploymentType { get; set; } = JiraDeploymentType.Cloud;

    /// <summary>
    /// REST API version to use. Data Center always uses v2, so this option is available only for Jira Cloud.
    /// </summary>
    /// <example>JiraCloudApiVersion.V2</example>
    [UIHint(nameof(DeploymentType), "", JiraDeploymentType.Cloud)]
    public JiraCloudApiVersion CloudApiVersion { get; set; } = JiraCloudApiVersion.V3;

    /// <summary>
    /// Atlassian account email used together with the API token in Basic authentication.
    /// Required only for Jira Cloud.
    /// </summary>
    /// <example>john.doe@company.com</example>
    [UIHint(nameof(DeploymentType), "", JiraDeploymentType.Cloud)]
    public string Email { get; set; }

    /// <summary>
    /// Personal Access Token (Data Center) or API token (Cloud).
    /// </summary>
    /// <example>1a2b3c4d5e6f7g8h9i0j1k2l3m4n5o6p7q8r9s0t1u2v3w4x5y6z</example>
    [DisplayFormat(DataFormatString = "Text")]
    [PasswordPropertyText]
    public string Token { get; set; }
}
