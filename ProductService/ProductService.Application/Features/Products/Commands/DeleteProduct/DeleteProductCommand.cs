﻿using MediatR;

namespace ProductService.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(int id) : IRequest<Unit>;
}
