using Newtonsoft.Json.Linq;

namespace Frends.Jira.GetIssue.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    internal Result(bool success, JToken data, Error error = null)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Jira issue or native search response. JQL returns the first page by default;
    /// use Cloud nextPageToken as Input.NextPageToken or Data Center startAt plus issue count as Input.StartAt for the next page.
    /// GetAllPages combines pages up to MaxIssues. Cloud API v3 descriptions are ADF objects.
    /// </summary>
    /// <example>{"issues":[{"key":"TT-1"}]}</example>
    public JToken Data { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }
}
