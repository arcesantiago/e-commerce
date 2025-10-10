using OrderService.Application.Contracts.Persistence;
using OrderService.Domain;
using OrderService.Infrastructure.Percistence;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderDbContext context) : base(context, context.Set<Order>())
        {
        }
    }
}
