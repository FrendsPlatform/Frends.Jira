using Newtonsoft.Json.Linq;

namespace Frends.Jira.GetIssue.Definitions;

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
    /// <example>{{"expand":"renderedFields,names,schema,operations,editmeta,changelog,versionedRepresentations","id":"151901"...</example>
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
