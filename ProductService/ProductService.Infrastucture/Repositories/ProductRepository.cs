using CleanArchitecture.Infrastucture.Repositories;
using ProductService.Application.Contracts.Persistence;
using ProductService.Domain;
using ProductService.Infrastucture.Percistence;

namespace ProductService.Infrastucture.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ProductDbContext context) : base(context)
        {
        }
    }
}
