using Microservices.Sample.Order.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microservices.Sample.Order.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Entities.Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Entities.Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Entities.Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Entities.Order> CreateAsync(Entities.Order order, CancellationToken cancellationToken = default);
        Task<Entities.Order> Updatesync(Entities.Order order, CancellationToken cancellationToken = default);
        Task<bool> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
