namespace ProductService.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandRequest
    {
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
