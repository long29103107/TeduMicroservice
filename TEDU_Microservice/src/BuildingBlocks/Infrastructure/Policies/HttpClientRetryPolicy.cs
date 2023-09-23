using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;

namespace Infrastructure.Policies;
public static class HttpClientRetryPolicy
{
    public static IHttpClientBuilder UseImmediateHttpRetryPolicy(this IHttpClientBuilder builder,int retryCount = 3)
    {
        return builder.AddPolicyHandler(ConfigureImmediateHttpRetry(retryCount));
    }

    public static IHttpClientBuilder UseLinearHttpRetryPolicy(this IHttpClientBuilder builder,int retryCount = 3, int fromSecond = 30)
    {
        return builder.AddPolicyHandler(ConfigureLinearHttpRetry(retryCount, fromSecond));
    }

    public static IHttpClientBuilder UseExponentialHttpRetryPolicy(this IHttpClientBuilder builder, int retryCount = 3)
    {
        return builder.AddPolicyHandler(ConfigureExponentialHttpRetry(retryCount));
    }

    public static IHttpClientBuilder UseCircuitBreakerPolicy(this IHttpClientBuilder builder, int eventAllowedBeforeBreaking = 3, int fromSeconds = 30)
    {
        return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy(3, 30));
    }

    public static IHttpClientBuilder ConfigureTimeoutPolicy(this IHttpClientBuilder builder, int seconds = 5)
    {
        return builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(seconds));
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy(int eventAllowedBeforeBreaking = 3, int fromSeconds = 30)
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: eventAllowedBeforeBreaking,
            durationOfBreak: TimeSpan.FromSeconds(fromSeconds));
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureImmediateHttpRetry(int retryCount)
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .RetryAsync(retryCount, (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
            });
    }

    private static IAsyncPolicy<HttpResponseMessage> ConfigureLinearHttpRetry(int retryCount, int fromSecond)
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(retryCount, retryAttemp => TimeSpan.FromSeconds(fromSecond),
            (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
            });
    }
    private static IAsyncPolicy<HttpResponseMessage> ConfigureExponentialHttpRetry(int retryCount)
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(retryCount, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
            (exception, retryCount, context) =>
            {
                Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to {exception}");
            });
    }
}
