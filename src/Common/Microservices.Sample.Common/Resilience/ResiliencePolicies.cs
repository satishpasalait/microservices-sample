using System.Net.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Microservices.Sample.Common.Resilience
{
    public static class ResiliencePolicies
    {
        //HTTP Retry Policy
        public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy(ILogger? logger = null)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        logger?.LogWarning(
                            "HTTP request failed. Delaying for {Delay}ms, then making retry {RetryCount}",
                            timespan.TotalMilliseconds,
                            retryCount
                        );
                    });
        }

        //HTTP Circuit Breaker Policy
        public static IAsyncPolicy<HttpResponseMessage> GetHttpCircuitBreakerPolicy(ILogger? logger = null)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, timespan) =>
                    {
                        logger?.LogWarning(
                            "Circuit breaker opened for {Duration}ms due to {Reason}",
                            timespan.TotalMilliseconds,
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString() ?? "Unknown"
                        );
                    },
                    onReset: () =>
                    {
                        logger?.LogInformation("Circuit breaker reset.");
                    },
                    onHalfOpen: () =>
                    {
                        logger?.LogInformation("Circuit breaker is half-open. Testing connection...");
                    });
        }

        //Combined HTTP Policy (Retry + Circuit Breaker)
        public static IAsyncPolicy<HttpResponseMessage> GetHttpResiliencePolicy(ILogger? logger = null)
        {
            var retryPolicy = GetHttpRetryPolicy(logger);
            var circuitBreakerPolicy = GetHttpCircuitBreakerPolicy(logger);
            return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        }

        //Database Retry Policy
        public static AsyncRetryPolicy GetDatabaseRetryPolicy(ILogger? logger = null)
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        logger?.LogWarning(
                            "Database operation failed. Delaying for {Delay}ms, then making retry {RetryCount}. Exception: {ExceptionMessage}",
                            timespan.TotalMilliseconds,
                            retryCount,
                            exception.Message
                        );
                    });
        }

        //Database Circuit Breaker Policy
        public static AsyncCircuitBreakerPolicy GetDatabaseCircuitBreakerPolicy(ILogger? logger = null)
        {
            return Policy
                .Handle<Exception>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(10),
                    minimumThroughput: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, timespan) =>
                    {
                        logger?.LogWarning(
                            "Database circuit breaker opened for {Duration}ms due to exception: {ExceptionMessage}",
                            timespan.TotalMilliseconds,
                            exception.Message
                        );
                    },
                    onReset: () =>
                    {
                        logger?.LogInformation("Database circuit breaker reset.");
                    },
                    onHalfOpen: () =>
                    {
                        logger?.LogInformation("Database circuit breaker is half-open. Testing connection...");
                    });
        }

        //Combined Database Policy (Retry + Circuit Breaker)
        public static AsyncPolicy GetDatabaseResiliencePolicy(ILogger? logger = null)
        {
            var retryPolicy = GetDatabaseRetryPolicy(logger);
            var circuitBreakerPolicy = GetDatabaseCircuitBreakerPolicy(logger);
            return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
        }
    }
}
