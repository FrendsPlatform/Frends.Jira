using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Jira.DeleteIssue.Definitions;

/// <summary>
/// Additional parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Whether to throw an error on failure.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool ThrowErrorOnFailure { get; set; } = true;

    /// <summary>
    /// Overrides the error message on failure. If `ThrowErrorOnFailure` is set to `true`, then the original exception will be wrapped in a new Exception with this error message.
    /// </summary>
    /// <example>Failed to delete Jira issue</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string ErrorMessageOnFailure { get; set; } = string.Empty;
}
