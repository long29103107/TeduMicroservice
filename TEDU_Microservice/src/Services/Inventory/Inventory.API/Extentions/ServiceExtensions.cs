using Infrastructure.Extensions;
using Inventory.API.Services;
using Inventory.API.Services.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.API.Extentions
{
    public static class ServiceExtensions
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.ConfigureHealthChecks();
            services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));
        }
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
            services.AddScoped<IInventoryService, InventoryService>();
        }

        private static void ConfigureHealthChecks(this IServiceCollection services)
        {
            var databaseSettings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings));
            services.AddHealthChecks()
                .AddMongoDb(databaseSettings.ConnectionString, "Mongo Health", HealthStatus.Degraded);
        }
    }
}
