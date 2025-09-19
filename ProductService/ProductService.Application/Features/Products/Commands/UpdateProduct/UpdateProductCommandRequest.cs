namespace ProductService.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandRequest
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
