using OrderService.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Application.Features.Orders.Queries.GetOrder
{
    public class OrderVm
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemVm> Items { get; set; } = new();
    }

    public class OrderItemVm
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
