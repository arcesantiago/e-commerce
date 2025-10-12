using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Exceptions;
using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(int Id, OrderStatus Status) : IRequest<bool>;
    public class UpdateOrderStatusCommandHandler(IOrderUnitOfWork orderUnitOfWork, ILogger<UpdateOrderStatusCommandHandler> logger) : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IOrderUnitOfWork _orderUnitOfWork = orderUnitOfWork;
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger = logger;

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderUnitOfWork.Orders.FindAsync(request.Id);
            if (order is null)
            {
                _logger.LogWarning($"Order {request.Id} not found");
                throw new NotFoundException(nameof(Order), request.Id);
            }

            order.Status = request.Status;
            _orderUnitOfWork.Orders.Update(order);
            await _orderUnitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"The order {order.Id} was updated successfully");
            return true;
        }
    }
}
