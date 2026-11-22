using System.ComponentModel.DataAnnotations;

namespace Notix.Api.Configuration;

/// <summary>Outgoing e-mail settings (Brevo SMTP, "Smtp" section). An empty Host means e-mails are written to the log.</summary>
public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = string.Empty;

    [Range(1, 65535)]
    public int Port { get; set; } = 587;

    public string User { get; set; } = string.Empty;

    /// <summary>Secret: the Brevo SMTP key.</summary>
    public string Password { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = "Notix";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(Host);
}
