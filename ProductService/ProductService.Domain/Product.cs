using ProductService.Domain.Common;

namespace ProductService.Domain
{
    public class Product : BaseDomainModel
    {
        public string Description { get; set; } 
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}