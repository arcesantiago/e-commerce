using OrderService.Application.Contracts.Persistence.Read;
using OrderService.Application.Contracts.Persistence.Write;
using OrderService.Domain;

namespace OrderService.Application.Contracts.Persistence
{
    public interface IOrderRepository : IReadRepository<Order>, IWriteRepository<Order>
    {
    }
}
