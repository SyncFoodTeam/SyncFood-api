using SyncFoodApi.Controllers.Products.DTO;
using SyncFoodApi.Controllers.Users.DTO;
using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.FoodContainers
{
    public static class ProductUtils
    {
        // Prend une liste de Product et renvoi l'équivalent en ProductPrivateDTO
        public static List<ProductPrivateDTO> GetProductsPrivate(List<Product> products)
        {
            List<ProductPrivateDTO> productsPrivateDTO = new List<ProductPrivateDTO>();

            foreach (Product product in products)
            {
                productsPrivateDTO.Add((ProductPrivateDTO)product);
            }

            return productsPrivateDTO;

           /* List<ProductPrivateDTO> productsPrivateDTO = new List<ProductPrivateDTO>();
            Dictionary<Product,int> ProductQtyDict = new Dictionary<Product,int>();

            // Première passe pour remplir le dictionnaire
            foreach (Product product in products)
            {
                if (ProductQtyDict.ContainsKey(product))
                {
                    ProductQtyDict[product] += 1;
                }

                else
                    ProductQtyDict.Add(product, 1);
            }

            // Deuxième passe pour remplir la liste de ProductPrivateDTO
            foreach (KeyValuePair<Product,int> productQty in ProductQtyDict) 
            {
                ProductPrivateDTO productPrivateDTO = (ProductPrivateDTO)productQty.Key;
                productPrivateDTO.Quantity = productQty.Value;
                productsPrivateDTO.Add(productPrivateDTO);
            }

            return productsPrivateDTO;*/
        }

        public static ProductPrivateDTO getProductPrivate(Product product, int productQuantity)
        {
            ProductPrivateDTO productPrivateDTO = (ProductPrivateDTO)product;
            productPrivateDTO.Quantity = productQuantity;
            return productPrivateDTO;
        }
    }
}
