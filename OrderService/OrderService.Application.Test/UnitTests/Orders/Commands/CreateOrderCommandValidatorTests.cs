using OrderService.Application.Features.Orders.Commands.CreateOrder;

namespace OrderService.Application.Test.UnitTests.Orders.Commands;
public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_RequestIsValid()
    {
        var createOrderCommandItemRequest = new List<CreateOrderCommandItemRequest>() { new CreateOrderCommandItemRequest(1, 2, 50) };
        var request = new CreateOrderCommandRequest("CUST-1", DateTime.UtcNow, createOrderCommandItemRequest);

        var result = _validator.Validate(new CreateOrderCommand(request));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_CustomerIdIsEmpty()
    {
        var createOrderCommandItemRequest = new List<CreateOrderCommandItemRequest>() { new CreateOrderCommandItemRequest(1, 2, 50) };
        var request = new CreateOrderCommandRequest(string.Empty, DateTime.UtcNow, createOrderCommandItemRequest);

        var result = _validator.Validate(new CreateOrderCommand(request));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("CustomerId"));
    }
}
