using Infrastructure.Identity;
using Common.Logging;
using Infrastructure.Middlewares;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;
using MMLib.SwaggerForOcelot;

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
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureOcelot(builder.Configuration);
    builder.Services.ConfigureCors(builder.Configuration);
    builder.Services.ConfigureAuthenticationHandler();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.UseSwagger();
        //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
        //    $"{builder.Environment.ApplicationName} v1"));
    }
    app.UseCors("CorsPolicy");

    app.UseMiddleware<ErrorWrappingMiddleware>();
   // app.UseAuthentication();
    app.UseRouting();

    //app.UseHttpsRedirection();

    ///app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/",context =>
        {
            //await context.Response.WriteAsync($"Hello hello");
            context.Response.Redirect("swagger/index.html");
            return Task.CompletedTask;
        });
    });

    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
        opt.OAuthClientId("tedu_microservices_swagger");
        opt.DisplayRequestDuration();
    });

    await app.UseOcelot();

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
        throw;
    Log.Fatal(ex, "Unhanded exception");
}
finally
{
    Log.Information("Shut down Product API complete");
    Log.CloseAndFlush();
}