namespace Microservices.Sample.Contracts.Events
{
    public record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
        public string ShippingAddress { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string ZipCode { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }
}
