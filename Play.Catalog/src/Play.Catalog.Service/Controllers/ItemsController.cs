using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Entities;

namespace Play.Catalog.Service.Controllers
{

    [ApiController]
    [Route("items")]


    public class ItemsController : ControllerBase
    {

        private readonly IItemRepository repository;

        public ItemsController(IItemRepository repository)
        {
            this.repository = repository;
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
            var item = await repository.GetItemAsync(id);
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
            return CreatedAtAction(nameof(GetByIDAsync), new { id = item.Id }, item);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingitem = await repository.GetItemAsync(id);
            if (existingitem == null)
            {
                return NotFound();
            }

            existingitem.Name = updateItemDto.Name;
            existingitem.Description = updateItemDto.Description;
            existingitem.Price = updateItemDto.Price;
            await repository.UpdateAsync(existingitem);
            return NoContent();
        }

        //Delete an item /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var existingitem = await repository.GetItemAsync(id);
            if (existingitem == null)
            {
                return NotFound();
            }

            await repository.RemoveAsync(id);
            return NoContent();
        }

    }
}