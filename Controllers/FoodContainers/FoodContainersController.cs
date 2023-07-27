using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.Models;
using SyncFoodApi.dbcontext;
using Microsoft.AspNetCore.Authorization;
using SyncFoodApi.Controllers.FoodContainers.DTO;

namespace SyncFoodApi.Controllers.FoodContainers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodContainersController : Controller
    {
        private readonly SyncFoodContext _context;

        public FoodContainersController(SyncFoodContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public ActionResult<FoodContainer> CreateFoodContainer(FoodContainerCreateDTO request)
        {

            Group group = _context.Groups.FirstOrDefault(x => x.Id == request.GroupId);
            if (group != null)
            {

                FoodContainer foodContainer = new FoodContainer()
                {
                    Name = request.Name,
                    Description = request.Description,
                    group = group
                };

                _context.FoodContainers.Add(foodContainer);
                _context.SaveChanges();

                return Ok((PrivateFoodContainerDTO)foodContainer);
            }

            else
                return NotFound();
        }

        [HttpPatch("edit")]
        public ActionResult<FoodContainer> EditFoodContainer(FoodContainerEditDTO request, int foodContainerID)
        {
            FoodContainer foodContainer = _context.FoodContainers.FirstOrDefault(x => x.Id == foodContainerID);

            if (foodContainer != null)
            {


                bool updateFoodContainer = !(request.Name == string.Empty && request.Description == string.Empty);

                if (updateFoodContainer)
                {
                    if (request.Name != string.Empty && request.Name != null)
                        foodContainer.Name = request.Name;

                    if (request.Description != string.Empty && request.Description != null)
                        foodContainer.Description = request.Description;

                    _context.FoodContainers.Update(foodContainer);
                    _context.SaveChanges();

                    return Ok((PrivateFoodContainerDTO)foodContainer);
                }

                else
                    return BadRequest();
            }

            else
                return NotFound();
        }

        [HttpDelete("delete")]
        public ActionResult<FoodContainer> DeleteFoodContainer(int foodContainerID)
        {
            FoodContainer foodContainer = _context.FoodContainers.FirstOrDefault(x => x.Id == foodContainerID);

            if (foodContainer != null)
            {
                _context.FoodContainers.Remove(foodContainer);
                _context.SaveChanges();

                return Ok((PrivateFoodContainerDTO)foodContainer);
            }

            else
                return NotFound();
        }

        [HttpPost("product/add")]
        public ActionResult<FoodContainer> FoodContainerAddProduct(int foodContainerID, int productID)
        {
            FoodContainer foodContainer = _context.FoodContainers.FirstOrDefault(x => x.Id == foodContainerID);
            Product product = _context.Products.FirstOrDefault(x => x.Id == productID);

            if (product != null && foodContainer != null)
            {
                foodContainer.Products.Add(product);
                _context.FoodContainers.Update(foodContainer);

                return Ok((PrivateFoodContainerDTO)foodContainer);
            }

            return NotFound();
        }

        [HttpPost("product/remove")]
        public ActionResult<FoodContainer> FoodContainerRemoveProduct(int foodContainerID, int productID)
        {
            FoodContainer foodContainer = _context.FoodContainers.FirstOrDefault(x => x.Id == foodContainerID);
            Product product = _context.Products.FirstOrDefault(x => x.Id == productID);

            if (product != null && foodContainer != null)
            {
                if (foodContainer.Products.Contains(product))
                {
                    foreach(Product currentProduct in foodContainer.Products)
                    {
                        if (currentProduct.Id == productID)
                        {
                            foodContainer.Products.Remove(currentProduct);
                        }
                    }

                    _context.FoodContainers.Update(foodContainer);
                    _context.SaveChanges();

                    return Ok((PrivateFoodContainerDTO)foodContainer);

                }

                else
                    return BadRequest();
            }

            else
                return NotFound();
        }

       


    }
}
