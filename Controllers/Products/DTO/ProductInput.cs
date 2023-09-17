using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Products.DTO
{
    public class ProductAddDTO
    {
        /*public required string Name { get; set; }*/
        public required float Price { get; set; }
        public required string BarCode { get; set; }
        /*public NutriScore NutriScore { get; set; }*/
        /*public float nutritionalValue { get; set; }*/
        public required int Quantity { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public required int FoodContainerID { get; set; }

    }

    public class ProductEditDTO
    {
        public required int ProductID { get; set; }
        public required int Quantity { get; set; }
    }

}
