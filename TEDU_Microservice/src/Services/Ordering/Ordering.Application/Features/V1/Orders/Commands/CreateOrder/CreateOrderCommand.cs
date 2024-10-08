﻿using AutoMapper;
using EventBus.Messages.IntegrationEvents.Events;
using MediatR;
using Ordering.Application.Common.Mappings;
using OrderDto = Ordering.Application.Common.Models.OrderDto;
using Ordering.Domain.Entities;
using Shared.Dtos.Order;
using Shared.SeedWord;

namespace Ordering.Application.Features.V1.Orders
{
    public class CreateOrderCommand : CreateOrUpdateCommand,IRequest<ApiResult<OrderDto>>,IMapFrom<Order>,
        IMapFrom<BasketCheckoutEvent>
    {
        public string Username { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateOrderDto, CreateOrderCommand>();
            profile.CreateMap<CreateOrderCommand, Order>();
            profile.CreateMap<BasketCheckoutEvent, CreateOrderCommand>();
        }
    }
}
