using System.ComponentModel.DataAnnotations;

namespace Notix.Api.Configuration;

/// <summary>Location of the technical and activity log files ("Logs" section).</summary>
public sealed class LogsOptions
{
    public const string SectionName = "Logs";

    /// <summary>Root folder for log files. Relative paths are resolved against the content root at startup.</summary>
    [Required]
    public string Root { get; set; } = string.Empty;
}
