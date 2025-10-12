namespace ProductService.Application.Contracts.Persistence
{
    public interface IProductUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}