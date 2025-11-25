using AutoMapper;
using MediatR;
using Microservices.Sample.Order.Application.DTOs;
using Microservices.Sample.Order.Application.Queries;
using Microservices.Sample.Order.Domain.Repositories;

namespace Microservices.Sample.Order.Application.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                return null;
            }
            return _mapper.Map<OrderDto>(order);
        }
    }
}
