using Microsoft.Extensions.Logging;
using OrderService.Application.Contracts.Persistence;
using OrderService.Application.Exceptions;
using OrderService.Application.Models;
using Polly;
using Polly.Retry;
using System.Net;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.Services
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductServiceClient> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(200 * retryAttempt),
                    (outcome, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning("Reintentando llamada a ProductService. Intento {RetryCount}", retryCount);
                    });
        }

        public async Task<ProductSnapshot> GetByIdAsync(int id)
        {
            var response = await _retryPolicy.ExecuteAsync(() =>
                _httpClient.GetAsync($"api/Products/{id}"));

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException(nameof(ProductSnapshot), id);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ProductSnapshot>();
        }
    }
}
