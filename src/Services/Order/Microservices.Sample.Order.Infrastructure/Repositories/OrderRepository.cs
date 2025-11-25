using Microsoft.EntityFrameworkCore;
using Microservices.Sample.Order.Domain.Entities;
using Microservices.Sample.Order.Domain.Repositories;
using Microservices.Sample.Order.Infrastructure.Data;
using OrderEntity = Microservices.Sample.Order.Domain.Entities.Order;

namespace Microservices.Sample.Order.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _dbContext;

        public OrderRepository(OrderDbContext context)
        {
            _dbContext = context;
        }

        public async Task<OrderEntity?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public async Task<IEnumerable<OrderEntity>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<OrderEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ToListAsync(cancellationToken);
        }

        public async Task<OrderEntity> CreateAsync(OrderEntity order, CancellationToken cancellationToken = default)
        {
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return order;
        }

        public async Task<OrderEntity> Updatesync(OrderEntity order, CancellationToken cancellationToken = default)
        {
            order.UpdatedAt = DateTime.UtcNow;
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return order;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, OrderStatus status, CancellationToken cancellationToken = default)
        {
            var order = await GetByIdAsync(id, cancellationToken);
            if (order == null) return false;

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order == null) return false;

            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
