using Microservices.Sample.Common.Resilience;
using Microservices.Sample.Order.Domain.Entities;
using Microservices.Sample.Order.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Polly;

namespace Microservices.Sample.Order.Infrastructure.Repositories
{
    public class ResilientOrderRepository : IOrderRepository
    {
        private readonly IOrderRepository _repository;
        private readonly IAsyncPolicy _resiliencePolicy;
        private readonly ILogger<ResilientOrderRepository> _logger;

        public ResilientOrderRepository(
            IOrderRepository repository, 
            ILogger<ResilientOrderRepository> logger)
        {
            _repository = repository;
            _resiliencePolicy = ResiliencePolicies.GetDatabaseResiliencePolicy(logger);
            _logger = logger;
        }

        public async Task<Domain.Entities.Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        { 
            return await _resiliencePolicy.ExecuteAsync(async () =>
                await _repository.GetByIdAsync(orderId, cancellationToken));
        }

        public async Task<IEnumerable<Domain.Entities.Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
                await _repository.GetByCustomerIdAsync(customerId, cancellationToken));
        }

        public async Task<IEnumerable<Domain.Entities.Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
                await _repository.GetAllAsync(cancellationToken));
        }

        public async Task<Domain.Entities.Order> CreateAsync(Domain.Entities.Order order, CancellationToken cancellationToken = default)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
                await _repository.CreateAsync(order, cancellationToken));
        }

        public async Task<Domain.Entities.Order> Updatesync(Domain.Entities.Order order, CancellationToken cancellationToken = default)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
                await _repository.Updatesync(order, cancellationToken));
        }

        public async Task<bool> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
                await _repository.UpdateStatusAsync(id, status, cancellationToken));
        }

        public async Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _resiliencePolicy.ExecuteAsync(async () =>
                await _repository.DeleteAsync(orderId, cancellationToken));
        }
    }
}
