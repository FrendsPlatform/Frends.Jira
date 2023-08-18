namespace Frends.Jira.AddIssueComment.Definitions;

/// <summary>
/// Options parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Throw an error on exception.
    /// If set to false, exception message can be found in Result.ErrorMessage.
    /// </summary>
    /// <example>true</example>
    public bool ThrowOnError { get; set; }
}