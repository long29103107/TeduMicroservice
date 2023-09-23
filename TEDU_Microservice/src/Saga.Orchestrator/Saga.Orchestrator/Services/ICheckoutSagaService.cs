using Shared.Dtos.Basket;

namespace Saga.Orchestrator.Services;

public interface ICheckoutSagaService
{
    Task<bool> CheckoutOrder(string username, BasketCheckoutDto basketCheckoutDto);
}
