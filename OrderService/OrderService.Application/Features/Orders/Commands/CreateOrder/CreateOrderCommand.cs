using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Contracts.Persistence;
using OrderService.Domain;
using OrderService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(CreateOrderCommandRequest CreateOrderCommandRequest) : IRequest<int>;
    public class CreateOrderCommandHandler(ILogger<CreateOrderCommandHandler> logger, IMapper mapper, IOrderUnitOfWork orderUnitOfWork, IProductServiceClient productServiceClient) : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CreateOrderCommandHandler> _logger = logger;
        private readonly IOrderUnitOfWork _orderUnitOfWork = orderUnitOfWork;
        private readonly IProductServiceClient _productServiceClient = productServiceClient;

        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request.CreateOrderCommandRequest);
            order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);
            order.Status = OrderStatus.Pending;

            foreach (var item in request.CreateOrderCommandRequest.Items)
            {
                var product = await _productServiceClient.GetByIdAsync(item.ProductId);

                if (product.Stock < 1)
                    throw new ValidationException("Out of stock");
                if (product.Price < 1)
                    throw new ValidationException("Invalid price");
            }

            await _orderUnitOfWork.Orders.AddAsync(order, cancellationToken);
            await _orderUnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"The product {order.Id} was created successfully");

            return order.Id;
        }
    }
}
