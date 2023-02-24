using Play.Catalog.Entities;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service{
    public static class Extensions{
        public static ItemDto asDto(this Item item){
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}