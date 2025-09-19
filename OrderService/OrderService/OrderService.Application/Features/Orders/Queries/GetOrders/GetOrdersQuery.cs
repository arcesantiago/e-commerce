using AutoMapper;
using MediatR;
using OrderService.Application.Contracts.Persistence;

namespace OrderService.Application.Features.Orders.Queries.GetOrders
{
    public record GetOrdersQuery() : IRequest<List<OrdersVm>>;
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrdersVm>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _repository;

        public GetOrdersQueryHandler(IMapper mapper, IOrderRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<OrdersVm>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _repository.GetAsync(includeString: "Items", disableTracking: false);
            return _mapper.Map<List<OrdersVm>>(orders);
        }
    }

}
