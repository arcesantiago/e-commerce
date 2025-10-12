namespace OrderService.Application.Contracts.Persistence
{
    public interface IOrderUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        IOrderItemRepository OrderItems { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
