namespace ProductService.Application.Features.Products.Queries.GetProduct
{
    public class ProductVm
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
