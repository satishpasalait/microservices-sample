using MediatR;
using Microservices.Sample.Order.Application.DTOs;
namespace Microservices.Sample.Order.Application.Commands
{
    public record CreateOrderCommand(
        Guid CustomerId,
        string CustomerName,
        string CustomerEmail,
        List<OrderItemDto> OrderItems,
        string ShippingAddress,
        string City,
        string State,
        string ZipCode,
        string Country
    ) : IRequest<Guid>;
}
