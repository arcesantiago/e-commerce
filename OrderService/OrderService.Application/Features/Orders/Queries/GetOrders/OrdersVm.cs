using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Orders.Queries.GetOrders
{
    public class OrdersVm
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemsVm> Items { get; set; } = new();
    }
    public class OrderItemsVm
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
