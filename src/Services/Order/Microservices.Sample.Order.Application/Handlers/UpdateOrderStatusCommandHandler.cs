using MassTransit;
using MediatR;
using Microservices.Sample.Contracts.Events;
using Microservices.Sample.Order.Application.Commands;
using Microservices.Sample.Order.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Microservices.Sample.Order.Application.Handlers
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

        public UpdateOrderStatusCommandHandler(
            IOrderRepository repository,
            IPublishEndpoint publishEndpoint,
            ILogger<UpdateOrderStatusCommandHandler> logger)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var result = await _repository.UpdateStatusAsync(request.Id, request.Status, cancellationToken);

            if (result)
            {
                await _publishEndpoint.Publish(new OrderStatusChangedEvent
                {
                    OrderId = request.Id,
                    Status = request.Status.ToString(),
                    ChangedAt = DateTime.UtcNow
                }, cancellationToken);
            }

            _logger.LogInformation("Order status updated: OrderId={OrderId}, NewStatus={NewStatus}, Success={Success}",
                request.Id, request.Status, result);

            return result;
        }
    }
}
