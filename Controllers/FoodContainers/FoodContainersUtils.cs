using SyncFoodApi.Controllers.FoodContainers.DTO;
using SyncFoodApi.Controllers.Groups.DTO;
using SyncFoodApi.Controllers.Products.DTO;
using SyncFoodApi.Controllers.Users.DTO;
using SyncFoodApi.dbcontext;
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

        // fonction pour vider le foodcontainer (utile pour la suppression en cascade)
        /*public static void Empty(SyncFoodContext _context, FoodContainer foodContainer)
        {
            foreach (Product product in foodContainer.Products)
            {
                Console.WriteLine(product.BarCode);
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            foodContainer.Products = new List<Product>();

            _context.SaveChanges();
        }*/
    }
}
