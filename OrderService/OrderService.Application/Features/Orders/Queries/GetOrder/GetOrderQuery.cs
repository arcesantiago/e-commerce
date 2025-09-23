using AutoMapper;
using MediatR;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Exceptions;
using OrderService.Domain;

namespace OrderService.Application.Features.Orders.Queries.GetOrder
{
    public record GetOrderQuery(int Id) : IRequest<OrderVm>;
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderQuery, OrderVm>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _repository;


        public GetOrderByIdQueryHandler(IMapper mapper, IOrderRepository repository)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OrderVm> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByAsync(x => x.Id == request.Id, new() { x => x.Items}, false);

            if (order is null)
                throw new NotFoundException(nameof(Order), request.Id);

            return _mapper.Map<OrderVm>(order);
        }
    }

}
