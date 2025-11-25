namespace Microservices.Sample.Order.Application.DTOs
{
    public record ProductInfoDto(
        Guid Id,
        string Name,
        decimal Price,
        int StockQuantity
    );
}
