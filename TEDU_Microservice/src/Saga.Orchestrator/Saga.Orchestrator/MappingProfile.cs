using AutoMapper;
using Shared.Dtos.Basket;
using Shared.Dtos.Inventory;
using Shared.Dtos.Order;

namespace Saga.Orchestrator;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BasketCheckoutDto, CreateOrderDto>();
        CreateMap<CartItemDto, SaleItemDto>();
    }
}
