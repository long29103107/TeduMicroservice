using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Basket.API.Extensions;
using Common.Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
Log.Information($"Start {builder.Environment.ApplicationName} Api up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.ConfigureServices();
    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.Configure <RouteOptions > (options => 
            options.LowercaseUrls = true);
    builder.Services.AddControllers();
    builder.Services.ConfigureHttpClientService();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    builder.Services.ConfigureMassTransit();
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new Basket.API.MappingProfile()));
    builder.Services.ConfigureGrpcServices();
    builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
    {
        var dataAccess = Assembly.GetExecutingAssembly();
        builder.RegisterAssemblyTypes(dataAccess)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        builder.RegisterAssemblyTypes(dataAccess)
            .Where(t => t.Name.EndsWith("Service"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    });
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
            $"{builder.Environment.ApplicationName} v1"));
    }
    app.UseRouting();
    //app.UseHttpsRedirection();

    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapDefaultControllerRoute();
    });

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if(type.Equals("StopTheHostException", StringComparison.Ordinal))
        throw;
    Log.Fatal(ex, "Unhanded exception");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} API complete");
    Log.CloseAndFlush();
}