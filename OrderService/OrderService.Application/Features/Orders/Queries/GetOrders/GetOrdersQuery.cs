using AutoMapper;
using MediatR;
using OrderService.Application.Contracts.Persistence;

namespace OrderService.Application.Features.Orders.Queries.GetOrders
{
    public record GetOrdersQuery() : IRequest<List<OrdersVm>>;
    public class GetOrdersQueryHandler(IMapper mapper, IOrderRepository orderRepository) : IRequestHandler<GetOrdersQuery, List<OrdersVm>>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IOrderRepository _orderRepository = orderRepository;
        public async Task<List<OrdersVm>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
                var orders = await _orderRepository.GetListAsync(
                includeProperties:
                [
                    o => o.Items
                ],
                //select: x => new Order
                //{
                //    Items = x.Items.Select(y => new OrderItem
                //    {
                //        Id = y.Id
                //    }).ToList()
                //},
                disableTracking: false,
                cancellationToken: cancellationToken);

            return _mapper.Map<List<OrdersVm>>(orders);
        }
    }
}
