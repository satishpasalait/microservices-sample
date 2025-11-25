using AutoMapper;
using Microservices.Sample.Order.Application.DTOs;
using OrderEntity = Microservices.Sample.Order.Domain.Entities.Order;
using OrderItemEntity = Microservices.Sample.Order.Domain.Entities.OrderItem;

namespace Microservices.Sample.Order.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderEntity, OrderDto>();
            CreateMap<OrderItemEntity, OrderItemDto>();
        }
    }
}
