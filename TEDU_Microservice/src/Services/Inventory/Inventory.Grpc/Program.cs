using Common.Logging;
using Serilog;
using Inventory.Grpc.Extensions;
using Inventory.Grpc.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);
Log.Information($"Start {builder.Environment.ApplicationName} up");

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Services.ConfigureMongoDbClient(builder.Configuration);
builder.Services.AddGrpc();
builder.Services.ConfigureHealthChecks();
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenLocalhost(5007, o => o.Protocols = HttpProtocol.Http2);
//});

var app = builder.Build();

app.UseRouting();
//app.UseHttpsRedirection();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapGrpcHealthChecksService();
    endpoints.MapGrpcService<InventoryService>();

    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Communication with gRPC endpoint must be made through");
    });
});

app.Run();
