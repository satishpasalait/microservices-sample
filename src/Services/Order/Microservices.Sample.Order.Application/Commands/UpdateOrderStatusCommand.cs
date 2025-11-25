using MediatR;
using Microservices.Sample.Order.Domain.Entities;

namespace Microservices.Sample.Order.Application.Commands
{
    public record UpdateOrderStatusCommand(
        Guid Id,
        OrderStatus Status
    ) : IRequest<bool>;
}
