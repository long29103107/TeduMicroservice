using AutoMapper;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.Dtos.Basket;
using Shared.Dtos.Inventory;
using Shared.Dtos.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Services;

public class CheckoutSageService : ICheckoutSagaService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CheckoutSageService(IBasketHttpRepository basketHttpRepository, IInventoryHttpRepository inventoryHttpRepository, IOrderHttpRepository orderHttpRepository, ILogger logger, IMapper mapper)
    {
        _basketHttpRepository = basketHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _orderHttpRepository = orderHttpRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> CheckoutOrder(string username, BasketCheckoutDto basketCheckoutDto)
    {
        //Get cart from BasketHttpRepository
        _logger.Information($"Start: Get Cart {username}");
        var cart = await _basketHttpRepository.GetBasket(username);
        if (cart == null) return false;
        _logger.Information($"End: Get Cart {username} success");

        //Create Order from OrderHttpRepository
        _logger.Information("Start: Create Order");
        
        var order = _mapper.Map<CreateOrderDto>(basketCheckoutDto);
        order.TotalPrice = cart.TotalPrice;
        var orderId = await _orderHttpRepository.CreateOrder(order);

        if (orderId < 0) return false;
        var addedOrder = await _orderHttpRepository.GetOrder(orderId); 

        _logger.Information($"End: Create Order success, Order Id: {orderId}, Document No: {addedOrder.DocumentNo}");

        var inventoryDocumentNo = new List<string>();
        bool result;
        try
        {
            //Sales Items from InventoryHttpRepository
            foreach (var item in cart.Items)
            {
                _logger.Information($"Start: Sale Item No {item.ItemNo} - Quantity {item.Quantity}");

                var salesOrder = new SalesProductDto(addedOrder.DocumentNo,item.Quantity);
                salesOrder.SetItemNo(item.ItemNo);

                var documentNo = await _inventoryHttpRepository.CreateSalesOrder(salesOrder);
                inventoryDocumentNo.Add(documentNo);

                _logger.Information($"End: Sale Item No {item.ItemNo} - Quantity {item.Quantity} - DocumentNo {documentNo}");
            }

            result = await _basketHttpRepository.DeleteBasket(username);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            await RollbackCheckoutOrder(username, addedOrder.Id, inventoryDocumentNo);
            result = false;
        }
        return result;
    }

    private async Task RollbackCheckoutOrder(string username, int orderId, List<string> inventoryDocumentNos)
    {
        _logger.Information($"Start: RollbackCheckoutOrder for username: {username}, " +
            $"order id: {orderId}" +
            $"inventory document nos: {String.Join(", ", inventoryDocumentNos)}");

        var deletedDocumentNos = new List<string>();
        //Delete Order by order's id, order's document no

        _logger.Information($"Start: Delete order id: {orderId}");
        await _orderHttpRepository.DeleteOrder(orderId);
        _logger.Information($"End: Delete order id: {orderId}");

        foreach (var documentNo in inventoryDocumentNos)
        {
            await _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
            deletedDocumentNos.Add(documentNo);
        }
        _logger.Information($"End: Deleted Inventory Document Nos: {String.Join(", ",inventoryDocumentNos)}");
    }
}
