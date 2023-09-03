using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Products.DTO
{
    // Produit générale
    /*public class ProductPublicDTO
    {
        public int Id { get; set; }
        //public required string Name { get; set; }
        public required int BarCode { get; set; }
        public required float Price { get; set; }
        //public NutriScore NutriScore { get; set; }
        //public float nutritionalValue { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public static explicit operator ProductPublicDTO(Product product)
        {
            return new ProductPublicDTO
            {
                Id = product.Id,
                //Name = product.Name,
                Price = product.Price,
                BarCode = product.BarCode,
                //NutriScore = product.NutriScore,
                //nutritionalValue = product.nutritionalValue,
                ExpirationDate = product.ExpirationDate,
                CreationDate = product.CreationDate
            };
        }
    }*/

    // Produit présent dans un foodcontainer ou une shoopingList
    public class ProductPrivateDTO
    {
        public int Id { get; set; }
        //public required string Name { get; set; }
        public required int BarCode { get; set; }
        public required float Price { get; set; }
        public int Quantity { get; set; }
        //public NutriScore NutriScore { get; set; }
        //public float nutritionalValue { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public static explicit operator ProductPrivateDTO(Product product)
        {
            return new ProductPrivateDTO
            {
                Id = product.Id,
                //Name = product.Name,
                Price = product.Price,
                BarCode = product.BarCode,
                //NutriScore = product.NutriScore,
                //nutritionalValue = product.nutritionalValue,
                ExpirationDate = product.ExpirationDate,
                CreationDate = product.CreationDate
            };
        }
    }
}
