namespace Notix.Api.Configuration;

/// <summary>Contact form settings ("Contact" section).</summary>
public sealed class ContactOptions
{
    public const string SectionName = "Contact";

    /// <summary>Address that receives the messages sent through the contact form.</summary>
    public string TeamEmail { get; set; } = string.Empty;
}
