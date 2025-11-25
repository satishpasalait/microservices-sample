using System;
using System.Collections.Generic;
using System.Text;

namespace Microservices.Sample.Order.Application.DTOs
{
    public record OrderItemDto(
        Guid Id,
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice
    );
}
