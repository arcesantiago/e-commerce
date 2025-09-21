using OrderService.Domain.Common;
using OrderService.Domain.Enums;

namespace OrderService.Domain
{
    public class Order : BaseDomainModel
    {
        public string CustomerId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
