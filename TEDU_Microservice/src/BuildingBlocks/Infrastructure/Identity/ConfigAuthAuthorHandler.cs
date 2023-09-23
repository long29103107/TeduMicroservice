using IdentityServer4.AccessTokenValidation;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.Configurations;
using System.Text.Json;

namespace Infrastructure.Identity;
public static class ConfigAuthAuthorHandler
{
    public static void ConfigureAuthenticationHandler(this IServiceCollection services)
    {
        var configuration = services.GetOptions<ApiConfigurationSettings>(nameof(ApiConfigurationSettings));

        if (configuration == null || string.IsNullOrEmpty(configuration.IssuerUri) || string.IsNullOrEmpty(configuration.ApiName)) throw new Exception("ApiConfigurationSettings is not configured !");

        var issuerUri = configuration.IssuerUri;
        Log.Information($"ApiConfigurationSettings Product  {JsonSerializer.Serialize(configuration)}");
        var apiName = configuration.ApiName;
        services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(opt =>
            {
                opt.Authority = issuerUri;
                opt.ApiName = apiName;
                opt.RequireHttpsMetadata = false;
                opt.SupportedTokens = SupportedTokens.Both;
            });
    }
    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(IdentityServerAuthenticationDefaults.AuthenticationScheme, policy =>
            {
                policy.AddAuthenticationSchemes(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        });
    }
}
