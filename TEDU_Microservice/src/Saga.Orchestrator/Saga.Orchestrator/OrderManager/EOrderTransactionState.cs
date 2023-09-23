namespace Saga.Orchestrator.OrderManager;

public enum EOrderTransactionState
{
    NotStarted,
    BasketGot,
    BasketGetFailed,
    BasketDeleted,
    BasketDeleteFailed,
    OrderCreated,
    OrderCreateFailed,
    OrderGot,
    OrderGetFailed,
    OrderDeleted,
    OrderDeleteFailed,
    InventoryUpdated,
    InventoryUpdateFailed,
    InvetoryRollback,
    RollbackInventory,
    InventoryRollback,
    InventoryRollbackFailed
}
