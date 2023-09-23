using Contracts.Saga.OrderManager;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services;
using Shared.Dtos.Basket;
using System.ComponentModel.DataAnnotations;

namespace Saga.Orchestrator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ISagaOrderManager<BasketCheckoutDto, OrderResponse> _sagaOrderManager;

    public CheckoutController(ISagaOrderManager<BasketCheckoutDto, OrderResponse> sagaOderManager)
    {
        _sagaOrderManager = sagaOderManager;
    }

    [HttpPost]
    [Route("{username}")]
    public OrderResponse CheckoutOrder([Required] string username, [FromBody] BasketCheckoutDto model)
    {
        model.Username = username;
        var result = _sagaOrderManager.CreateOrder(model);
        return result;
    }
}
