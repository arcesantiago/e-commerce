using AutoMapper;
using MediatR;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Exceptions;
using OrderService.Domain;

namespace OrderService.Application.Features.Orders.Queries.GetOrder
{
    public record GetOrderQuery(int Id) : IRequest<OrderVm>;
    public class GetOrderByIdQueryHandler(IMapper mapper, IOrderUnitOfWork orderUnitOfWork) : IRequestHandler<GetOrderQuery, OrderVm>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IOrderUnitOfWork _orderUnitOfWork = orderUnitOfWork;
        public async Task<OrderVm> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderUnitOfWork.Orders.GetEntityAsync(
                x => x.Id == request.Id, 
                includeProperties: [x => x.Items], 
                disableTracking: false, 
                cancellationToken: cancellationToken);

            return order is null ? throw new NotFoundException(nameof(Order), request.Id) : _mapper.Map<OrderVm>(order);
        }
    }
}
