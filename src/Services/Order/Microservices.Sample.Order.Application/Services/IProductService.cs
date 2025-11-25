
using Microservices.Sample.Order.Application.DTOs;

namespace Microservices.Sample.Order.Application.Services
{
    public interface IProductService
    {
        Task<ProductInfo?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);
    }
}
