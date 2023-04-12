
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Entities;
using Play.Common;
using MassTransit;
using Play.Catalog.Contracts;

namespace Play.Catalog.Service.Controllers
{

    [ApiController]
    [Route("items")]


    public class ItemsController : ControllerBase
    {

        private readonly IRepository<Item> repository;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> repository, IPublishEndpoint publishEndpoint)
        {
            this.repository = repository;
            this.publishEndpoint = publishEndpoint;
        }
        
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> Get()
        {
            var items = (await repository.GetAllAsync())
                .Select(item => item.asDto());

            return items;
        }


        [HttpGet("{id}")]

        public async Task<ActionResult<ItemDto>> GetByIDAsync(Guid id)
        {
            var item = await repository.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return item.asDto();
        }


        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto itemDto)
        {
            var item = new Item
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await repository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIDAsync), new { id = item.Id }, item);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingitem = await repository.GetAsync(id);
            if (existingitem == null)
            {
                return NotFound();
            }

            existingitem.Name = updateItemDto.Name;
            existingitem.Description = updateItemDto.Description;
            existingitem.Price = updateItemDto.Price;
            await repository.UpdateAsync(existingitem);
            await publishEndpoint.Publish(new CatalogItemUpdated(existingitem.Id, existingitem.Name, existingitem.Description));
            return NoContent();
        }

        //Delete an item /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var existingitem = await repository.GetAsync(id);
            if (existingitem == null)
            {
                return NotFound();
            }

            await repository.RemoveAsync(id);
            await publishEndpoint.Publish(new CatalogItemRemoved(id));
            return NoContent();
        }

    }
}