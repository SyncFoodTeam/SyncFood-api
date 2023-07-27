using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Products.DTO
{
    public class ProductCreateDTO
    {
        public required string Name { get; set; }
        public required float Price { get; set; }
        public required int BarCode { get; set; }
        public NutriScore NutriScore { get; set; }
        public float nutritionalValue { get; set; }
        public DateTime ExpirationDate { get; set; }

    }

}
