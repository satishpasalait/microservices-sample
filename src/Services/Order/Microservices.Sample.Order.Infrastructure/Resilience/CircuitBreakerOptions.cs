
namespace Microservices.Sample.Order.Infrastructure.Resilience
{
    public class CircuitBreakerOptions
    {
        public int HandledEventsAllowedBeforeBreaking { get; set; } = 5;
        public int DurationOfBreakSeconds { get; set; } = 30;
    }
}
