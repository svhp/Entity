namespace Play.Inventory.Service.Dtos
{
    public record GrantItemsDto(Guid Id, Guid CatalogItemId, int Quantity);
    public record InventoryItemDto(Guid CatalogItemId, string name, string description, int Quantity, DateTimeOffset AcquiredDate);
    public record CatalogItemDto(Guid id, string Name, string Description);
}