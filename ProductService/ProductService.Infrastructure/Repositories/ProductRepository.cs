using CleanArchitecture.Infrastructure.Repositories;
using ProductService.Application.Contracts.Persistence;
using ProductService.Domain;
using ProductService.Infrastructure.Percistence;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ProductDbContext context) : base(context)
        {
        }
    }
}
