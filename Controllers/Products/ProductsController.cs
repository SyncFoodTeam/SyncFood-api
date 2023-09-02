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
using SyncFoodApi.Controllers.FoodContainers.DTO;
using Microsoft.AspNetCore.Authorization;

namespace SyncFoodApi.Controllers.Products
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly SyncFoodContext _context;

        public ProductsController(SyncFoodContext context)
        {
            _context = context;
        }


        [HttpPost("add")]
        public ActionResult<FoodContainerPrivateDTO> addProduct(ProductAddDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
            {
                return Unauthorized();
            }

            FoodContainer foodcontainer = _context.FoodContainers.Include(x => x.group).Include(x => x.Products).FirstOrDefault(x => x.Id == request.FoodContainerID);

            if (foodcontainer == null)
                return NotFound("foodcontainer introuvable");

            Product product = _context.Products.FirstOrDefault(x => x.FoodContainer == foodcontainer && x.BarCode == request.BarCode);

            if (product == null)
            {
                foodcontainer.Products.Add(new Product
                {
                    /*Name = request.Name,*/
                    Price = request.Price,
                    BarCode = request.BarCode,
                    /*NutriScore = request.NutriScore,*/
                    ExpirationDate = request.ExpirationDate
                });
                _context.FoodContainers.Update(foodcontainer);
            }

            else
            {
                product.Quantity += request.Quantity;
                _context.Products.Update(product);
            }

            _context.SaveChanges();

            return Ok((FoodContainerPrivateDTO)foodcontainer);


        }

    }

}
