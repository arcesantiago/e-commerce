using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Exceptions;
using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(int Id, OrderStatus Status) : IRequest<bool>;
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

        public UpdateOrderStatusCommandHandler(IOrderRepository repository, ILogger<UpdateOrderStatusCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.Id);
            if (order is null)
            {
                _logger.LogWarning($"Order {request.Id} not found");
                throw new NotFoundException(nameof(Order), request.Id);
            }

            order.Status = request.Status;
            await _repository.UpdateAsync(order);

            _logger.LogInformation($"The order {order.Id} was updated successfully");
            return true;
        }
    }
}
