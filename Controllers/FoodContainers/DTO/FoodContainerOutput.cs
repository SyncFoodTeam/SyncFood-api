using SyncFoodApi.Controllers.Groups.DTO;
using SyncFoodApi.Controllers.Products.DTO;
using SyncFoodApi.Models;
using static SyncFoodApi.Controllers.FoodContainers.ProductUtils;

namespace SyncFoodApi.Controllers.FoodContainers.DTO
{
    public class FoodContainerPrivateDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public List<ProductPrivateDTO> Products { get; set; } = new List<ProductPrivateDTO>();
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public static explicit operator FoodContainerPrivateDTO(FoodContainer foodContainer)
        {
            return new FoodContainerPrivateDTO
            {
                Id = foodContainer.Id,
                Name = foodContainer.Name,
                Description = foodContainer.Description,
                Products = GetProductsPrivate(foodContainer.Products),
                CreationDate = foodContainer.CreationDate
            };    
        }
    }

    public class FoodContainerPrivateLiteDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public static explicit operator FoodContainerPrivateLiteDTO(FoodContainer foodContainer)
        {
            return new FoodContainerPrivateLiteDTO
            {
                Id = foodContainer.Id,
                Name = foodContainer.Name,
                Description = foodContainer.Description,
                CreationDate = foodContainer.CreationDate
            };
        }
    }

}
