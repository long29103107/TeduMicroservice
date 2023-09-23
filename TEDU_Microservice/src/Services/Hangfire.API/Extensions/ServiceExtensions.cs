using Contracts.ScheduledJobs;
using Contracts.Services;
using Hangfire.API.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.ScheduledJobs;
using Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configurations;

namespace Hangfire.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var hangFireSettings = configuration.GetSection(nameof(HangFireSettings)).Get<HangFireSettings>();
        services.AddSingleton(hangFireSettings);

        var smtpSettings = configuration.GetSection(nameof(EmailSMTPSettings)).Get<EmailSMTPSettings>();
        services.AddSingleton(smtpSettings);

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<IScheduledJobService, HangfireService>();
        services.AddTransient<IBackgroundJobService, BackgroundJobService>();
        services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));
        
        return services;
    }

    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var hangFireSettings = services.GetOptions<HangFireSettings>(nameof(HangFireSettings));
        services.AddHealthChecks()
            .AddMongoDb(hangFireSettings.Storage.ConnectionString, name: "MongoDb Hangfire Health", failureStatus: HealthStatus.Degraded);
    }
}

