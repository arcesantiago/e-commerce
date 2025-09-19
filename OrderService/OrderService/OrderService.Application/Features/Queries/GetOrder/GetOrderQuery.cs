using AutoMapper;
using MediatR;
using OrderService.Application.Contracts.Persistence;

namespace OrderService.Application.Features.Queries.GetOrder
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
            var order = await _repository.GetByIdWithDetailsAsync(request.Id);
            return _mapper.Map<OrderVm>(order);
        }
    }

}
