namespace Notix.Api.Configuration;

/// <summary>Database seeding settings ("Seed" section).</summary>
public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    /// <summary>When true, demo users, materials, reviews and an order are inserted (Development only).</summary>
    public bool DemoData { get; set; }
}
