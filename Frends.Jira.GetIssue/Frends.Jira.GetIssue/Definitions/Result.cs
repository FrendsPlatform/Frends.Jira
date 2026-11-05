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

    // TODO Pagination
    /// <summary>
    /// Issue (search by ID or key) or search results (search by JQL) returned by Jira, as JToken.
    /// With Jira Cloud API v3 the description is an ADF object, with API v2 and Data Center it is a string.
    /// JQL search returns only the first page of results. Jira Cloud returns a nextPageToken, Data Center returns total and startAt.
    /// </summary>
    /// <example>{{"expand":"renderedFields,names,schema,operations,editmeta,changelog,versionedRepresentations","id":"151901"...</example>
    public JToken Data { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }
}
