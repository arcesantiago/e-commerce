namespace ProductService.Application.Features.Products.Queries.GetPagedProductsList
{
    public class PagedProductsListVm
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
