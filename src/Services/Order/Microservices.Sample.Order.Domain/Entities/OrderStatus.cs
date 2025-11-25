using System;
using System.Collections.Generic;
using System.Text;

namespace Microservices.Sample.Order.Domain.Entities
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
