using System.ComponentModel.DataAnnotations;

namespace Frends.Jira.DeleteIssue.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Specify the issue ID or key to delete.
    /// </summary>
    /// <example>TT-1, 123</example>
    [Required(ErrorMessage = "Issue ID or key is required.")]
    public string IdOrKey { get; set; }
}
