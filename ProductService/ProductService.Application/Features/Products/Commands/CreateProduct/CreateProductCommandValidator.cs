using FluentValidation;

namespace ProductService.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator() 
        {
            RuleFor(p => p.CreateProductCommandRequest.Description)
                .NotNull().WithMessage("{Description} no puede ser nulo");
            RuleFor(p => p.CreateProductCommandRequest.Price)
                .GreaterThan(0).WithMessage("{Price} debe ser positivo");
        }
    }
}
