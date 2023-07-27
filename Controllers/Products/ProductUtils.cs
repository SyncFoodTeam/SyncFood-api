using SyncFoodApi.Controllers.Products.DTO;
using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Products
{
    public static class ProductUtils
    {
        public static List<ProductPublicDTO> getProductsPublic(List<Product> products)
        {
            List<ProductPublicDTO> productsPublic = new List<ProductPublicDTO>();

            foreach (Product product in products)
            {
                productsPublic.Add((ProductPublicDTO)product);
            }

            return productsPublic;
        }
    }
}
