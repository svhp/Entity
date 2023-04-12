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
        private readonly IRepository<InventoryItem> inventoryRepository;
        private readonly IRepository<CatalogItem> catalogRepository;

        public ItemsController(IRepository<InventoryItem> repository, IRepository<CatalogItem> catalogRepository)
        {
            this.inventoryRepository = repository;
            this.catalogRepository = catalogRepository;
        }
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {

            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var inventoryitemsEntities = await inventoryRepository.GetAllAsync(item => item.UserId == userId);
            var itemIds = inventoryitemsEntities.Select(item => item.CatalogItemId);
            var catalogItems = await catalogRepository.GetAllAsync(item => itemIds.Contains(item.Id));

            var inventoryItemDtos = inventoryitemsEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
               
            });

            return Ok(inventoryItemDtos);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var InventoryItem = await inventoryRepository.GetAsync(
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
                await inventoryRepository.CreateAsync(InventoryItem);
            }
            else
            {
                InventoryItem.Quantity += grantItemsDto.Quantity;
                await inventoryRepository.UpdateAsync(InventoryItem);
            }
            return Ok();
        }



    }
}