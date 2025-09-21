using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Models;

namespace OrderService.API.Test.IntegrationTests.FakeServices
{
    public class FakeProductServiceClient : IProductServiceClient
    {
        Task<ProductSnapshot> IProductServiceClient.GetByIdAsync(int id)
        {
            return Task.FromResult(new ProductSnapshot
            {
                Id = id,
                Description = "Fake Product",
                Stock = 10,
                Price = 50
            });
        }
    }
}
