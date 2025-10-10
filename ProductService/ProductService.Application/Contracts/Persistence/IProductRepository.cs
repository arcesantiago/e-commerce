using ProductService.Application.Contracts.Persistence.Read;
using ProductService.Application.Contracts.Persistence.Write;
using ProductService.Domain;

namespace ProductService.Application.Contracts.Persistence
{
    public interface IProductRepository : IReadRepository<Product>, IWriteRepository<Product>
    {
    }
}
