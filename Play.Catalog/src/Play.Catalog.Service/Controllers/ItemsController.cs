using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Play.Catalog.Service.Dtos;


namespace Play.Catalog.Service.Controllers
{

    [ApiController]
    [Route("items")]


    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new(){

            new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)

        };


        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }


        [HttpGet("{id}")]

        public ActionResult<ItemDto> GetByID(Guid id)
        {
            var item = items.Where(item => item.id == id).SingleOrDefault();
            if (item == null)
            {
                 return NotFound();
            }
            return Ok(item);
        }

    
        [HttpPost]
        public ActionResult<ItemDto> Post(CreateItemDto itemDto)
        {
            ItemDto item = new ItemDto(Guid.NewGuid(), itemDto.Name, itemDto.Description, itemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);
            return CreatedAtAction(nameof(GetByID), new { id = item.id }, item);
        }


        [HttpPut("{id}")]
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto){

            var existingItem = items.Where(item => item.id == id).SingleOrDefault();
            if (existingItem == null)
            {
                return NotFound();
            }

            ItemDto updatedItem = existingItem with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            int index = items.FindIndex(existingItem => existingItem.id == id);
            items[index] = updatedItem;

            return NoContent();
        }

        //Delete an item /items/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            int index = items.FindIndex(existingItem => existingItem.id == id);

            if (index == -1)
            {
                return NotFound();
            }

            items.RemoveAt(index);
            return NoContent();
        }

    }
}