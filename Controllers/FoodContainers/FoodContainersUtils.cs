using SyncFoodApi.Controllers.FoodContainers.DTO;
using SyncFoodApi.Controllers.Groups.DTO;
using SyncFoodApi.Controllers.Products.DTO;
using SyncFoodApi.Controllers.Users.DTO;
using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.FoodContainers
{
    public static class FoodcontainersUtils
    {
        // Prend une liste de FoodContainers et renvoi l'équivalent en FoodContainerPrivateLiteDTO
        public static List<FoodContainerPrivateLiteDTO> getFoodContainersLiteDTO(List<FoodContainer> foodContainers)
        {
            List<FoodContainerPrivateLiteDTO> foodContainersPrivateliteDTO = new List<FoodContainerPrivateLiteDTO>();
            foreach (FoodContainer foodContainer in foodContainers)
            {
                foodContainersPrivateliteDTO.Add((FoodContainerPrivateLiteDTO)foodContainer);
            }

            return foodContainersPrivateliteDTO;
        }
    }
}
