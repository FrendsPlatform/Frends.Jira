using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace Frends.Jira.UpdateIssue.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Specify the issue ID or key to update.
    /// </summary>
    /// <example>TT-1, 123</example>
    public string IdOrKey { get; set; }

    /// <summary>
    /// Modified content as JToken.
    /// </summary>
    /// <example>{ "fields": { "summary": "new summary"} }</example>
    [DisplayFormat(DataFormatString = "Json")]
    public JToken Content { get; set; }
}