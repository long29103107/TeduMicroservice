using Shared.Dtos.Order;


namespace Saga.Orchestrator.HttpRepository.Interfaces;

public interface IOrderHttpRepository
{
    Task<int> CreateOrder(CreateOrderDto order);
    Task<OrderDto> GetOrder(int id);
    Task<bool> DeleteOrder(int id);
    Task<bool> DeleteOrderByDocumentNo(string documentNo);
}
