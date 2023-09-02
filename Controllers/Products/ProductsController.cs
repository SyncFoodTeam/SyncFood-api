using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.Models;
using SyncFoodApi.dbcontext;
using static SyncFoodApi.Controllers.Users.UserUtils;
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

        [HttpPost("add")]
        public ActionResult<ProductPrivateDTO> addProduct(ProductAddDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
            {
                return Unauthorized();
            }

            FoodContainer foodcontainer = _context.FoodContainers.Include(x => x.group).FirstOrDefault(x => x.Id == request.FoodContainerID);

            if (foodcontainer == null)
                return NotFound("foodcontainer introuvable");

            if ()
        }*/

    }

}
