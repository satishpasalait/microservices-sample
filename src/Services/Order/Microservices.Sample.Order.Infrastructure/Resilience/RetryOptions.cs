namespace Microservices.Sample.Order.Infrastructure.Resilience
{
    public class RetryOptions
    {
        public int MaxRetries { get; set; } = 3;
        public int BaseDelaySeconds { get; set; } = 2;
    }
}
