using Microsoft.EntityFrameworkCore;
using OrderService.Application.Contracts.Persistence;
using OrderService.Domain;
using OrderService.Infrastructure.Percistence;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderDbContext context) : base(context)
        {
        }
        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders!
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
