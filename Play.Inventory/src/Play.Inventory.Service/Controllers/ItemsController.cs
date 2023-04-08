using Microsoft.AspNetCore.Mvc;
using Plau.Inventory.Service.Clients;
using Play.Common;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> repository;
        private readonly CatalogClient catalogClient;

        public ItemsController(IRepository<InventoryItem> repository, CatalogClient catalogClient)
        {
            this.repository = repository;
            this.catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {

            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await catalogClient.GetCatalogItemAsync();
            var inventoryitemsEntities = await repository.GetAllAsync(item => item.UserId == userId);

            var inventoryItemDtos = inventoryitemsEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
               
            });

            return Ok(inventoryItemDtos);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var InventoryItem = await repository.GetAsync(
                item => item.UserId == grantItemsDto.Id && item.CatalogItemId == grantItemsDto.CatalogItemId);
            if (InventoryItem is null)
            {
                InventoryItem = new InventoryItem
                {
                    UserId = grantItemsDto.Id,
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };
                await repository.CreateAsync(InventoryItem);
            }
            else
            {
                InventoryItem.Quantity += grantItemsDto.Quantity;
                await repository.UpdateAsync(InventoryItem);
            }
            return Ok();
        }



    }
}