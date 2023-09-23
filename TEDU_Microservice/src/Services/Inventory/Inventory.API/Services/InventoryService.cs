using AutoMapper;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Inventory.API.Entities;
using Inventory.API.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.Dtos.Inventory;
using Shared.SeedWord;

namespace Inventory.API.Services
{
    public class InventoryService : MongoDbRepository<InventoryEntry>, IInventoryService
    {
        private readonly IMapper _mapper;
        public InventoryService(IMongoClient client, MongoDbSettings settings, IMapper mapper) : base(client, settings)
        {
            _mapper = mapper;
        }
        public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
        {
            var entities = await FindAll()
                .Find(x => x.ItemNo.Equals(itemNo))
                .ToListAsync();
            var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
            return result;
        }

        public async Task<PageList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query)
        {
            var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
            var filterItemNo = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, query.ItemNo());
            if (!string.IsNullOrEmpty(query.SearchTerm))
                filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, query.SearchTerm);

            var andFilter = filterItemNo & filterSearchTerm;
            var pageList = await Collection.PaginatedListAsync(andFilter, query.PageNumber, query.PageSize);
            var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pageList);
            var result = new PageList<InventoryEntryDto>(items, pageList.GetMetaData().TotalItems, query.PageNumber, query.PageSize);
            return result;
        }

        public async Task<InventoryEntryDto> GetByIdAsync(string id)
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(x => x.Id, id);
            var entity = await FindAll().Find(filter).FirstOrDefaultAsync();

            var result = _mapper.Map<InventoryEntryDto>(entity);
            return result;
        }

        public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model)
        {
            var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                ItemNo = itemNo,
                Quantity = model.Quantity,
                DocumentType = model.DocumentType
            };
            await CreateAsync(itemToAdd);
            var result = _mapper.Map<InventoryEntryDto>(itemToAdd);
            return result;
        }

        public async Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model)
        {
            var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                ItemNo = itemNo,
                ExternalDocumentNo = model.ExtenalDocumentNo,
                Quantity = model.Quantity * -1,
                DocumentType = model.DocumentType
            };

            await CreateAsync(itemToAdd);

            var result = _mapper.Map<InventoryEntryDto>(itemToAdd);

            return result;
        }

        public async Task DeleteByDocumentNoAsync(string documentNo)
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, documentNo);
            await Collection.DeleteOneAsync(filter);
        }

        public async Task<string> SalesOrderAsync(SalesOrderDto model)
        {
            var documentNo = Guid.NewGuid().ToString();
            foreach(var saleItem in model.SalesItems)
            {
                var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
                {
                    DocumentNo = documentNo,
                    ItemNo = saleItem.ItemNo,
                    ExternalDocumentNo = model.OrderNo,
                    Quantity = saleItem.Quantity * -1,
                    DocumentType = saleItem.DocumentType
                };

                await CreateAsync(itemToAdd);
            }
            return documentNo;
        }
    }
}
