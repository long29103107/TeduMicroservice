using Infrastructure.Extensions;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.Dtos.Order;
using Shared.SeedWord;

namespace Saga.Orchestrator.HttpRepository;

public class OrderHttpRepository : IOrderHttpRepository
{
    private readonly HttpClient _client;

    public OrderHttpRepository(HttpClient client)
    {
        _client = client;
    }

    public async Task<int> CreateOrder(CreateOrderDto order)
    {
        var response = await _client.PostAsJsonAsync("order", order);
        if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            return -1;

        var orderResponse = await response.ReadContentAs<ApiSuccessResult<OrderDto>>();
        return orderResponse.Data.Id;
    }

    public async Task<bool> DeleteOrder(int id)
    {
        var response = await _client.DeleteAsync($"order/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
    {
        var response = await _client.DeleteAsync($"document-no/{documentNo}");
        return response.IsSuccessStatusCode;
    }

    public async Task<OrderDto> GetOrder(int id)
    {
        var order = await _client.GetFromJsonAsync<ApiSuccessResult<OrderDto>>($"order/{id}");
        return order.Data;
    }
}
