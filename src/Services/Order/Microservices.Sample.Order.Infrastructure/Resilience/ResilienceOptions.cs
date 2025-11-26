namespace Microservices.Sample.Order.Infrastructure.Resilience
{
    public class ResilienceOptions
    {
        public const string SectionName = "Resilience";
        public RetryOptions Retry { get; set; } = new();
        public CircuitBreakerOptions CircuitBreaker { get; set; } = new();
    }
}
