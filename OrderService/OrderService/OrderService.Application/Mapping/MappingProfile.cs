using AutoMapper;
using OrderService.Application.Features.Commands.CreateOrder;
using OrderService.Application.Features.Queries.GetOrder;
using OrderService.Application.Features.Queries.GetOrders;
using OrderService.Domain;

namespace OrderService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderVm>();
            CreateMap<Order, OrdersVm>();
            CreateMap<OrderItem, OrderItemVm>();
            CreateMap<OrderItem, OrderItemsVm>();
            CreateMap<CreateOrderCommandRequest, Order>();
            CreateMap<CreateOrderCommandItemRequest, OrderItem>();
        }
    }
}
