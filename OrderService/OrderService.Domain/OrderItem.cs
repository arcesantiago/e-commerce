using OrderService.Domain.Common;

namespace OrderService.Domain
{
    public class OrderItem : BaseDomainModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
