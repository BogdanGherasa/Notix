namespace Notix.Api.Configuration;

/// <summary>Stripe Checkout settings ("Stripe" section). Both values are secrets.</summary>
public sealed class StripeOptions
{
    public const string SectionName = "Stripe";

    public string SecretKey { get; set; } = string.Empty;

    public string WebhookSecret { get; set; } = string.Empty;
}
