namespace Notix.Api.Configuration;

/// <summary>Google reCAPTCHA v2 settings ("Recaptcha" section).</summary>
public sealed class RecaptchaOptions
{
    public const string SectionName = "Recaptcha";

    /// <summary>Public key, sent to the browser through /api/config/public.</summary>
    public string SiteKey { get; set; } = string.Empty;

    /// <summary>Secret: used only server-side for siteverify.</summary>
    public string SecretKey { get; set; } = string.Empty;
}
