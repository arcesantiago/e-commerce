using OrderService.Application.Contracts.Persistence;
using OrderService.Domain;
using OrderService.Infrastructure.Percistence;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : RepositoryBase<Order>(context), IOrderRepository
    {
    }
}
