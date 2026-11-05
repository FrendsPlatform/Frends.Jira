namespace Frends.Jira.UpdateIssue.Definitions;

/// <summary>
/// Jira deployment type.
/// </summary>
public enum JiraDeploymentType
{
    /// <summary>
    /// Jira Cloud (hosted by Atlassian, e.g. https://yourcompany.atlassian.net).
    /// </summary>
    Cloud,

    /// <summary>
    /// Jira Data Center or Server (self-hosted). Supports only REST API v2.
    /// </summary>
    DataCenter,
}

/// <summary>
/// REST API version used with Jira Cloud.
/// </summary>
public enum JiraCloudApiVersion
{
    /// <summary>
    /// REST API v3.
    /// </summary>
    V3,

    /// <summary>
    /// REST API v2.
    /// </summary>
    V2,
}
