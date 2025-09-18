using OrderService.Domain;

namespace OrderService.Application.Contracts.Persistence
{
    public interface IOrderItemRepository : IAsyncRepository<OrderItem>
    {
    }
}
