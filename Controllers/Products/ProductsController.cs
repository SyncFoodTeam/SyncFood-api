﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.Models;
using SyncFoodApi.dbcontext;
using static SyncFoodApi.Controllers.Users.UserUtils;
//using static SyncFoodApi.Controllers.Products.ProductUtils;
using SyncFoodApi.Controllers.Products.DTO;
using SyncFoodApi.Controllers.FoodContainers.DTO;
using Microsoft.AspNetCore.Authorization;
using Azure.Core;

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
        public ActionResult<ProductPrivateDTO> addProduct(ProductAddDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();


            if (request.Quantity <= 0)
                return BadRequest("quantitée doit être > 0");

            FoodContainer foodcontainer = _context.FoodContainers.Include(x => x.group).Include(x => x.Products).FirstOrDefault(x => x.Id == request.FoodContainerID);

            if (foodcontainer == null)
                return NotFound("foodcontainer introuvable");

            Product product = _context.Products.Include(x => x.FoodContainer).FirstOrDefault(x => x.FoodContainer == foodcontainer && x.BarCode == request.BarCode);

            if (product == null)
            {
                product = new Product
                {
                    /*Name = request.Name,*/
                    Price = request.Price,
                    BarCode = request.BarCode,
                    Quantity = request.Quantity,
                    /*NutriScore = request.NutriScore,*/
                    ExpirationDate = request.ExpirationDate
                };

                // Console.WriteLine(product.Quantity);

                foodcontainer.Products.Add(product);
                _context.FoodContainers.Update(foodcontainer);
            }

            else
            {
                product.Quantity += request.Quantity;
                Console.WriteLine(product.Quantity);
                _context.Products.Update(product);
            }

            _context.SaveChanges();

            return Ok((ProductPrivateDTO)product);


        }


        [HttpPatch("edit")]
        public ActionResult<ProductPrivateDTO> editProduct(ProductEditDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            if (request.Quantity <= 0)
                return BadRequest("quantitée doit être > 0");

            Product product = _context.Products.FirstOrDefault(x => x.Id == request.ProductID);

            if (product == null)
                return NotFound("Le produit est introuvable");

            product.Quantity = request.Quantity;
            _context.Products.Update(product);
            _context.SaveChanges();

            return Ok((ProductPrivateDTO)product);

        }


        [HttpDelete("delete/{productID}")]
        public ActionResult<ProductPrivateDTO> deleteProduct(int productID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();


            Product product = _context.Products.FirstOrDefault(x => x.Id == productID);

            if (product == null)
                return NotFound("Produit introuvable");

            // supprimer le produit

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Ok((ProductPrivateDTO)product);


        }

    }

}
