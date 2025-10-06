namespace OrderService.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public DateTimeOffset OrderDate { get; set; }
        public List<CreateOrderCommandItemRequest> Items { get; set; } = new();
    }

    public class CreateOrderCommandItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
