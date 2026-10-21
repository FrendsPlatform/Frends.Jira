using Newtonsoft.Json.Linq;

namespace Frends.Jira.CreateIssue.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Request content as JToken.
    /// </summary>
    /// <example>{{ "id": "151959", "key": "TT-87", "self": "https://test.jira.fi/jira/rest/api/latest/issue/151959" }}</example>
    public JToken Data { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }

    internal Result(bool success, JToken data, Error error = null)
    {
        Success = success;
        Data = data;
        Error = error;
    }
}
