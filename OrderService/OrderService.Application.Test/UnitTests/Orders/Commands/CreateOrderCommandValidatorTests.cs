using OrderService.Application.Features.Orders.Commands.CreateOrder;

namespace OrderService.Application.Test.UnitTests.Orders.Commands;
public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_RequestIsValid()
    {
        var request = new CreateOrderCommandRequest
        {
            CustomerId = "CUST-1",
            OrderDate = DateTime.UtcNow,
            Items = new List<CreateOrderCommandItemRequest>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 50 }
            }
        };

        var result = _validator.Validate(new CreateOrderCommand(request));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Fail_When_CustomerIdIsEmpty()
    {
        var request = new CreateOrderCommandRequest
        {
            CustomerId = "",
            Items = new List<CreateOrderCommandItemRequest>
            {
                new() { ProductId = 1, Quantity = 2, UnitPrice = 50 }
            }
        };

        var result = _validator.Validate(new CreateOrderCommand(request));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("CustomerId"));
    }
}
