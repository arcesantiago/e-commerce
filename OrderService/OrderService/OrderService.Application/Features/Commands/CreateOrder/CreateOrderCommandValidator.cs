using FluentValidation;

namespace OrderService.Application.Features.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CreateOrderCommandRequest.CustomerId)
                .NotEmpty().WithMessage("The CustomerId is required");

            RuleForEach(x => x.CreateOrderCommandRequest.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId)
                    .GreaterThan(0).WithMessage("The ProductId must be greater than 0");

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("The quantity must be greater than 0");

                items.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0).WithMessage("The price must be positive");
            });
        }
    }
}
