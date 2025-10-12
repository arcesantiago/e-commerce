using OrderService.Application.Contracts.Persistence;
using OrderService.Infrastructure.Percistence;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderUnitOfWork : UnitOfWorkBase<OrderDbContext>, IOrderUnitOfWork
    {
        public IOrderRepository Orders { get; }

        public IOrderItemRepository OrderItems {get;}

        public OrderUnitOfWork(
            OrderDbContext context,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository
        ) : base(context)
        {
            Orders = orderRepository;
            OrderItems = orderItemRepository;
        }
    }
}