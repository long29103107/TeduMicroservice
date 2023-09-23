using Grpc.HealthCheck;
using Infrastructure.Extensions;
using Inventory.Grpc.Repositories;
using Inventory.Grpc.Repositories.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Grpc.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            services.AddSingleton(mongoDbSettings);
            
            return services;
        }

        private static string GetMongoConnectionString(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
                throw new ArgumentNullException("MongoDbSettings is not confugured");
            var databaseName = settings.DatabaseName;
            var mongoDbConnectionString = settings.ConnectionString + "/" + databaseName + "?authSource=admin";    
            return mongoDbConnectionString;
        }
        public static void ConfigureMongoDbClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(GetMongoConnectionString(services, configuration)));
            services.AddScoped(x => x.GetService<MongoClient>()?.StartSession());
            services.AddScoped<IInvensitoryRepository, InvensitoryRepository>();
        }

        public static void ConfigureHealthChecks(this IServiceCollection services)
        {
            var databaseSettings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings));
            services.AddSingleton<HealthServiceImpl>();
            services.AddHostedService<StatusService>();
            services.AddHealthChecks()
                .AddMongoDb(databaseSettings.ConnectionString, "Inventory MongoDb Health", HealthStatus.Degraded)
                .AddCheck("Inventory Grpc Health",() => HealthCheckResult.Healthy());
        }
    }
}
