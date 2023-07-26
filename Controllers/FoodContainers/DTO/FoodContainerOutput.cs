using SyncFoodApi.Controllers.Groups.DTO;
using SyncFoodApi.Controllers.Products.DTO;
using SyncFoodApi.Models;
using static SyncFoodApi.Controllers.FoodContainers.FoodContainerUtils;

namespace SyncFoodApi.Controllers.FoodContainers.DTO
{
    public class PrivateFoodContainerDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public List<ProductPrivateDTO> Products { get; set; } = new List<ProductPrivateDTO>();
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public static explicit operator PrivateFoodContainerDTO(FoodContainer foodContainer)
        {
            return new PrivateFoodContainerDTO
            {
                Id = foodContainer.Id,
                Name = foodContainer.Name,
                Description = foodContainer.Description,
                Products = GetProductsPrivate(foodContainer.Products),
                CreationDate = foodContainer.CreationDate
            };    
        }
    }

}
