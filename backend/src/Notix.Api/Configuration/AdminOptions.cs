namespace Notix.Api.Configuration;

/// <summary>The single administrator account created at startup if no admin exists ("Admin" section).</summary>
public sealed class AdminOptions
{
    public const string SectionName = "Admin";

    public string Email { get; set; } = string.Empty;

    /// <summary>Secret: set in User Secrets (local) or in the server environment file.</summary>
    public string Password { get; set; } = string.Empty;
}
