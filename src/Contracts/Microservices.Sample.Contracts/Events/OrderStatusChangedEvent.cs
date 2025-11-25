using System;
using System.Collections.Generic;
using System.Text;

namespace Microservices.Sample.Contracts.Events
{
    public record OrderStatusChangedEvent
    {
        public Guid OrderId { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime ChangedAt { get; init; }
    }
}
