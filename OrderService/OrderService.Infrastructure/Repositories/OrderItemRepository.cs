using OrderService.Application.Contracts.Persistence;
using OrderService.Domain;
using OrderService.Infrastructure.Percistence;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderItemRepository : RepositoryBase<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(OrderDbContext context) : base(context, context.Set<OrderItem>())
        {
        }
    }
}
