using Microservices.Sample.Order.Domain.Entities;

namespace Microservices.Sample.Order.Application.DTOs
{
    public record OrderDto (
        Guid Id,
        Guid CustomerId,
        string CustomerName,
        string CustomerEmail,
        List<OrderItemDto> OrderItems,
        decimal TotalAmount,
        OrderStatus Status,
        string ShippingAddress,
        string City,
        string State,
        string ZipCode,
        string Country,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
