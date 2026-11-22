using Microsoft.Extensions.Options;

namespace Notix.Api.Configuration;

/// <summary>Binds every configuration section to its typed options class.</summary>
public static class OptionsServiceCollectionExtensions
{
    public static IServiceCollection AddNotixOptions(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var contentRoot = environment.ContentRootPath;

        services.AddOptions<AppOptions>()
            .Bind(configuration.GetSection(AppOptions.SectionName))
            .PostConfigure(o => o.BaseUrl = o.BaseUrl.TrimEnd('/'))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<StorageOptions>()
            .Bind(configuration.GetSection(StorageOptions.SectionName))
            .PostConfigure(o => o.Root = ResolvePath(contentRoot, o.Root))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<LogsOptions>()
            .Bind(configuration.GetSection(LogsOptions.SectionName))
            .PostConfigure(o => o.Root = ResolvePath(contentRoot, o.Root))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AdminOptions>().Bind(configuration.GetSection(AdminOptions.SectionName));
        services.AddOptions<StripeOptions>().Bind(configuration.GetSection(StripeOptions.SectionName));
        services.AddOptions<RecaptchaOptions>().Bind(configuration.GetSection(RecaptchaOptions.SectionName));
        services.AddOptions<ContactOptions>().Bind(configuration.GetSection(ContactOptions.SectionName));
        services.AddOptions<SeedOptions>().Bind(configuration.GetSection(SeedOptions.SectionName));

        return services;
    }

    /// <summary>
    /// Writes the effective configuration to the log at startup.
    /// Secrets are never printed: only whether they are set.
    /// </summary>
    public static void LogNotixConfiguration(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Notix.Configuration");
        var sp = app.Services;

        var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
        var storage = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
        var logs = sp.GetRequiredService<IOptions<LogsOptions>>().Value;
        var admin = sp.GetRequiredService<IOptions<AdminOptions>>().Value;
        var stripe = sp.GetRequiredService<IOptions<StripeOptions>>().Value;
        var recaptcha = sp.GetRequiredService<IOptions<RecaptchaOptions>>().Value;
        var smtp = sp.GetRequiredService<IOptions<SmtpOptions>>().Value;
        var contact = sp.GetRequiredService<IOptions<ContactOptions>>().Value;
        var seed = sp.GetRequiredService<IOptions<SeedOptions>>().Value;

        var connectionString = app.Configuration.GetConnectionString("Default");

        logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
        logger.LogInformation("ConnectionStrings:Default = {ConnectionString}", string.IsNullOrWhiteSpace(connectionString) ? "(missing)" : connectionString);
        logger.LogInformation("App:BaseUrl = {BaseUrl}", appOptions.BaseUrl);
        logger.LogInformation("Storage:Root = {StorageRoot}", storage.Root);
        logger.LogInformation("Logs:Root = {LogsRoot}", logs.Root);
        logger.LogInformation("Admin: Email = {AdminEmail}, Password = {AdminPassword}", Plain(admin.Email), Secret(admin.Password));
        logger.LogInformation("Stripe: SecretKey = {StripeSecretKey}, WebhookSecret = {StripeWebhookSecret}", Secret(stripe.SecretKey), Secret(stripe.WebhookSecret));
        logger.LogInformation("Recaptcha: SiteKey = {RecaptchaSiteKey}, SecretKey = {RecaptchaSecretKey}", Plain(recaptcha.SiteKey), Secret(recaptcha.SecretKey));
        logger.LogInformation(
            "Smtp: Host = {SmtpHost}, Port = {SmtpPort}, User = {SmtpUser}, Password = {SmtpPassword}, From = {FromName} <{FromEmail}>, Mode = {SmtpMode}",
            Plain(smtp.Host), smtp.Port, Plain(smtp.User), Secret(smtp.Password), smtp.FromName, Plain(smtp.FromEmail),
            smtp.IsConfigured ? "SMTP" : "log only");
        logger.LogInformation("Contact:TeamEmail = {TeamEmail}", Plain(contact.TeamEmail));
        logger.LogInformation("Seed:DemoData = {DemoData}", seed.DemoData);
    }

    private static string ResolvePath(string contentRoot, string path) =>
        string.IsNullOrWhiteSpace(path) ? path : Path.GetFullPath(Path.Combine(contentRoot, path));

    private static string Plain(string value) => string.IsNullOrWhiteSpace(value) ? "(empty)" : value;

    private static string Secret(string value) => string.IsNullOrWhiteSpace(value) ? "(not set)" : "(set)";
}
