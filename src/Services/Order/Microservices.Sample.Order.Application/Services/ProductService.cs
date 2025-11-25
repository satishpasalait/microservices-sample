using Microservices.Sample.Order.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Polly.CircuitBreaker;

namespace Microservices.Sample.Order.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductService> _logger;

        public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductInfo?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/products/{productId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to retrieve product with ID {ProductId}. Status Code: {StatusCode}", productId, response.StatusCode);
                    return null;
                }

                var productDto = await response.Content
                    .ReadFromJsonAsync<ProductInfoDto>(cancellationToken: cancellationToken);

                if (productDto == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", productId);
                    return null;
                }

                return new ProductInfo
                (
                    productDto.Id,
                    productDto.Name,
                    productDto.Price,
                    productDto.StockQuantity
                );
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, "Circuit breaker is open. Unable to retrieve product with ID {ProductId}.", productId);
                throw new InvalidOperationException("Product service is currently unavailable. Please try again later.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while retrieving product with ID {ProductId}.", productId);
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timed out while retrieving product with ID {ProductId}.", productId);
                throw new InvalidOperationException("The request to the product service timed out. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product with ID {ProductId}.", productId);
                throw;
            }
        }
    }
}
