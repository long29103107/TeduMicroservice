using Inventory.API.Entities;
using Shared.Enums.Inventory;
using Inventory.API.Extentions;
using Shared.Configurations;

namespace Inventory.API.Persistence;

public class InventoryDbSeed
{
    public async Task SeedDataAsync(MongoDB.Driver.IMongoClient mongoClient, MongoDbSettings settings)
    {
        var databaseName = settings.DatabaseName;
        var database = mongoClient.GetDatabase(databaseName);
        var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");

        if(await inventoryCollection.EstimatedDocumentCountAsync() == 0)
        {
            await inventoryCollection.InsertManyAsync(GetPreconfiguredInventoryEntries());
        }    
    }

    private IEnumerable<InventoryEntry> GetPreconfiguredInventoryEntries()
    {
        return new List<InventoryEntry>
        {
            new InventoryEntry()
            {
                Quantity = 10,
                DocumentNo = Guid.NewGuid().ToString(),
                ItemNo = "Lotus",
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                DocumentType = EDocumentType.Purchase
            }
        };
    }
}
