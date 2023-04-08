using MongoDB.Driver;
using Play.Catalog.Entities;

namespace Play.Catalog.Service.Repositories
{

    public class ItemRepository : IItemRepository
    {
        private const string collectionName = "items";
        private readonly IMongoCollection<Item>? dbCollection;

        private readonly FilterDefinitionBuilder<Item>? filterBuilder = Builders<Item>.Filter;

        public ItemRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<Item>(collectionName);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<Item> GetItemAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task CreateAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            await dbCollection.InsertOneAsync(item);
        }
        public async Task UpdateAsync(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "Item cannot be null");
            }
            FilterDefinition<Item> filter = filterBuilder.Eq(ExistingEntity => ExistingEntity.Id, item.Id);
            await dbCollection.ReplaceOneAsync(filter, item);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

    }
}