using System.ComponentModel.DataAnnotations;

namespace Notix.Api.Configuration;

/// <summary>Location of uploaded files ("Storage" section). Must be outside the public web folder.</summary>
public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>Root folder for uploads. Relative paths are resolved against the content root at startup.</summary>
    [Required]
    public string Root { get; set; } = string.Empty;
}
