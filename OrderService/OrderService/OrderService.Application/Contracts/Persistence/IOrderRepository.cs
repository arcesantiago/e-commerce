using OrderService.Domain;

namespace OrderService.Application.Contracts.Persistence
{
    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<Order?> GetByIdWithDetailsAsync(int id);
    }
}
