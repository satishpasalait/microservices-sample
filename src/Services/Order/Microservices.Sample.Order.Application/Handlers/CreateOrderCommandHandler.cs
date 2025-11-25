using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microservices.Sample.Order.Application.Commands;
using Microservices.Sample.Order.Domain.Repositories;
using Microservices.Sample.Order.Application.Services;
using Microservices.Sample.Order.Domain.Entities;
using Microservices.Sample.Contracts.Events;

namespace Microservices.Sample.Order.Application.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _repository;
        private readonly IProductService _productService;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IOrderRepository repository,
            IProductService productService,
            IPublishEndpoint publishEndpoint,
            ILogger<CreateOrderCommandHandler> logger
        )
        {
            _repository = repository;
            _productService = productService;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }


        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in request.OrderItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId, cancellationToken);
                
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {item.ProductId} not found.");
                }

                if (product.StockQuantity < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}.");
                }

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * item.Quantity
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;
            }

            var order = new Domain.Entities.Order
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                OrderItems = orderItems,
                TotalAmount = totalAmount,
                ShippingAddress = request.ShippingAddress,
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode,
                Country = request.Country,
                CreatedAt = DateTime.UtcNow,
            };

            var createdOrder = await _repository.CreateAsync(order, cancellationToken);

            await _publishEndpoint.Publish(new OrderCreatedEvent
            {
                OrderId = createdOrder.Id,
                CustomerId = createdOrder.CustomerId,
                CustomerName = createdOrder.CustomerName,
                CustomerEmail = createdOrder.CustomerEmail,
                TotalAmount = createdOrder.TotalAmount,
                ShippingAddress = createdOrder.ShippingAddress,
                City = createdOrder.City,
                State = createdOrder.State,
                ZipCode = createdOrder.ZipCode,
                Country = createdOrder.Country,
                CreatedAt = createdOrder.CreatedAt
            }, cancellationToken);

            _logger.LogInformation("Order {OrderId} created successfully for Customer {CustomerId}", createdOrder.Id, createdOrder.CustomerId);
            return createdOrder.Id;
        }
    }
}
