using Play.Catalog.Entities;

namespace Play.Catalog.Service.Repositories
{
    public interface IItemRepository
    {
        Task CreateAsync(Item item);
        Task<IReadOnlyCollection<Item>> GetAllAsync();
        Task<Item> GetItemAsync(Guid id);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(Item item);
    }
}