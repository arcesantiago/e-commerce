using ProductService.Application.Contracts.Persistence;
using ProductService.Infrastructure.Percistence;

namespace ProductService.Infrastructure.Repositories
{
    public class ProductUnitOfWork : UnitOfWorkBase<ProductDbContext>, IProductUnitOfWork
    {
        public IProductRepository Products { get; }
        public ProductUnitOfWork(
            ProductDbContext context,
            IProductRepository productRepository
        ) : base(context)
        {
            Products = productRepository;
        }
    }
}