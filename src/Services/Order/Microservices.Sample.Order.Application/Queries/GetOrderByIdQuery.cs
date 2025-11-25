using MediatR;
using Microservices.Sample.Order.Application.DTOs;

namespace Microservices.Sample.Order.Application.Queries
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;
}
