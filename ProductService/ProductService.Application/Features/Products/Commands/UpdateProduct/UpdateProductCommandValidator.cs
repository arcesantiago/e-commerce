using FluentValidation;
using ProductService.Application.Features.Products.Commands.CreateProduct;

namespace ProductService.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(p => p.UpdateProductCommandRequest.Description)
                .NotNull().WithMessage("{Description} no puede ser nulo");
            RuleFor(p => p.UpdateProductCommandRequest.Price)
                .GreaterThan(0).WithMessage("{Price} debe ser positivo");
        }
    }
}
