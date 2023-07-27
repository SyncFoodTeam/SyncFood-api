using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.Models;
using SyncFoodApi.dbcontext;
using static SyncFoodApi.Controllers.Products.ProductUtils;
using SyncFoodApi.Controllers.Products.DTO;

namespace SyncFoodApi.Controllers.Products
{
    public class ProductsController : Controller
    {
        private readonly SyncFoodContext _context;

        public ProductsController(SyncFoodContext context)
        {
            _context = context;
        }

        /*
                [HttpPost("create")]
                public ActionResult<Product> createProduct()*/

        [HttpGet("findsyncfood")]
        public ActionResult<List<Product>> findProductOnSyncFood(string productName)
        {
            List<Product> products = _context.Products.Where(x => x.Name.ToLower() == productName.ToLower()).ToList();

            if (products.Count > 0)
            {
                return Ok(getProductsPublic(products));
            }
            
            else
                return NotFound();
        }


        [HttpGet("findopenfood")]
        public ActionResult<List<ProductPublicDTO>> findProductOnOpenFood(string productName)
        {
            List<ProductPublicDTO> products = new List<ProductPublicDTO>();
            // TODO interroger l'api openFood
            return products;
        }

 /*       [HttpPost("create")]
        public ActionResult<Product> createProduct(ProductCreateDTO request){
        {

        }
*/

    }
}
