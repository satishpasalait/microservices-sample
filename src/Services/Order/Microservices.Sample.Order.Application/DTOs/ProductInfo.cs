namespace Microservices.Sample.Order.Application.DTOs
{
    public record ProductInfo(
        Guid Id,
        string Name,
        decimal Price,
        int StockQuantity
    );
}
