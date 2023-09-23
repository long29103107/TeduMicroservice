using Saga.Orchestrator.Services;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.OrderManager;
using Contracts.Saga.OrderManager;
using Shared.Dtos.Basket;
using Common.Logging;
using Infrastructure.Policies;

namespace Saga.Orchestrator.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<ICheckoutSagaService, CheckoutSageService>();
        services.AddTransient<ISagaOrderManager<BasketCheckoutDto, OrderResponse>, SagaOrderManager>();
        services.AddTransient<LoggingDelegatingHandler>();
        return services;
    }

    public static IServiceCollection ConfigureHttpRespository(this IServiceCollection services)
    {
        services.AddScoped<IOrderHttpRepository, OrderHttpRepository>();
        services.AddScoped<IBasketHttpRepository, BasketHttpRepository>();
        services.AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();
        return services;
    }

    public static void ConfigureClient(this IServiceCollection services)
    {
        ConfigureOrderHttpClient(services);
        ConfigureBasketHttpClient(services);
        ConfigureInventoryHttpClient(services);
    }
    public static void ConfigureOrderHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IOrderHttpRepository, OrderHttpRepository>("OrderAPI", (sp, cl) =>
        {
            cl.BaseAddress = new Uri("http://localhost:5005/api/v1/");
        }).AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseExponentialHttpRetryPolicy();

        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("OrderAPI"));
    }

    public static void ConfigureBasketHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketAPI", (sp, cl) =>
        {
            cl.BaseAddress = new Uri("http://localhost:5004/api/");
        }).AddHttpMessageHandler<LoggingDelegatingHandler>()
            .UseImmediateHttpRetryPolicy();

        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("BasketAPI"));
    }

    public static void ConfigureInventoryHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventoryAPI", (sp, cl) =>
        {
            cl.BaseAddress = new Uri("http://localhost:5006/api/");
        }).AddHttpMessageHandler<LoggingDelegatingHandler>()
             .UseExponentialHttpRetryPolicy();

        services.AddScoped(sp => sp.GetService<IHttpClientFactory>().CreateClient("InventoryAPI"));
    }

}