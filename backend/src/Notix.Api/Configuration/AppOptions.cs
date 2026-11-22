using System.ComponentModel.DataAnnotations;

namespace Notix.Api.Configuration;

/// <summary>General application settings ("App" section).</summary>
public sealed class AppOptions
{
    public const string SectionName = "App";

    /// <summary>Public base URL of the site, without a trailing slash (used in e-mail links and Stripe redirects).</summary>
    [Required, Url]
    public string BaseUrl { get; set; } = string.Empty;
}
