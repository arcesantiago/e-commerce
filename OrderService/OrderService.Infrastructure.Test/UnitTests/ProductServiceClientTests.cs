using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using OrderService.Application.Exceptions;
using OrderService.Application.Models;
using OrderService.Infrastructure.Services;
using System.Net;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.Test.UnitTests;
public class ProductServiceClientTests
{
    private readonly Mock<ILogger<ProductServiceClient>> _loggerMock = new();

    private HttpClient CreateHttpClient(HttpResponseMessage responseMessage)
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://fake-product-service")
        };
    }

    [Fact]
    public async Task GetProductAsync_ShouldReturnProductSnapshot_WhenProductExists()
    {
        // Arrange
        var dto = new ProductSnapshot
        {
            Description = "Laptop",
            Price = 1200.50m,
            Stock = 10
        };

        var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(dto)
        });

        var client = new ProductServiceClient(httpClient, _loggerMock.Object);

        // Act
        var result = await client.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Id, result.Id);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.Price, result.Price);
        Assert.Equal(dto.Stock, result.Stock);
    }

    [Fact]
    public async Task GetProductAsync_ShouldThrow_WhenNotFound()
    {
        // Arrange
        var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = new ProductServiceClient(httpClient, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => client.GetByIdAsync(99));
    }

    [Fact]
    public async Task GetProductAsync_ShouldThrow_WhenServerError()
    {
        // Arrange
        var httpClient = CreateHttpClient(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var client = new ProductServiceClient(httpClient, _loggerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetByIdAsync(1));
    }
}
