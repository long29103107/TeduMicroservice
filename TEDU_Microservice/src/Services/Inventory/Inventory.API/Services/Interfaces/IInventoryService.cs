using Contracts.Domains.Interfaces;
using Inventory.API.Entities;
using Shared.Dtos.Inventory;
using Shared.SeedWord;
using System.Threading.Tasks;

namespace Inventory.API.Services.Interfaces
{
    public interface IInventoryService : IMongoDbRepositoryBase<InventoryEntry>
    { 
        Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo);
        Task<PageList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query);
        Task<InventoryEntryDto> GetByIdAsync(string id);
        Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model);
        Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model);
        Task DeleteByDocumentNoAsync(string documentNo);

        Task<string> SalesOrderAsync(SalesOrderDto model);
    }
}
