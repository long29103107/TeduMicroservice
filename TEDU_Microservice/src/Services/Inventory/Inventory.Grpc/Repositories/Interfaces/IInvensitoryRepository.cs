using Contracts.Domains.Interfaces;
using Inventory.Grpc.Entities;

namespace Inventory.Grpc.Repositories.Interfaces
{
    public interface IInvensitoryRepository : IMongoDbRepositoryBase<InventoryEntry>
    {
        Task<int> GetStockQuantity(string itemNo);
    }
}
