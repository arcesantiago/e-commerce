using OrderService.Application.Models;

namespace OrderService.Application.Contracts.Persistence
{
    public interface IProductServiceClient
    {
        Task<ProductSnapshot> GetByIdAsync(int id);
    }
}
