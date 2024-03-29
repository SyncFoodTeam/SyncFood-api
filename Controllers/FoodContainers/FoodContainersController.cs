﻿using System;
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
using static SyncFoodApi.Controllers.Users.UserUtils;
using static SyncFoodApi.Controllers.SyncFoodUtils;
using Microsoft.Extensions.Localization;
using SyncFoodApi.Controllers.Groups;
using System.Configuration;

namespace SyncFoodApi.Controllers.FoodContainers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodContainersController : Controller
    {
        private readonly SyncFoodContext _context;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<GroupsController> _Group_Localizer;
        private readonly IStringLocalizer<FoodContainersController> _FoodContainer_Localizer;

        public FoodContainersController(SyncFoodContext context, IConfiguration configuration, IStringLocalizer<GroupsController> groupLocalizer, IStringLocalizer<FoodContainersController> foodContainerLocalizer)
        {
            _context = context;
            _configuration = configuration;
            _Group_Localizer = groupLocalizer;
            _FoodContainer_Localizer = foodContainerLocalizer;
        }

        [HttpPost("create")]
        public ActionResult<FoodContainer> CreateFoodContainer(FoodContainerCreateDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            Group group = _context.Groups.Include(x => x.Members).FirstOrDefault(x => x.Id == request.GroupId);

            if (group == null)
                return NotFound(_Group_Localizer["group.notfound"]);

            if (!AllowedName(request.Name))
                return BadRequest(_FoodContainer_Localizer["invalid.foodcontainername"]);

            if (_context.FoodContainers.Include(x => x.group).Any(x => x.group == group && x.Name.ToLower() == request.Name.ToLower()))
                return Conflict(_FoodContainer_Localizer["foodcontainer.alreadyexist"]);

            if (!group.Members.Contains(user))
                return BadRequest();

            FoodContainer newFoodContainer = new FoodContainer()
            {
                Name = request.Name,
                Description = request.Description,
                group = group
            };

            _context.FoodContainers.Add(newFoodContainer);
            _context.SaveChanges();

            return Ok((FoodContainerPrivateDTO)newFoodContainer);
        }

        [HttpPatch("edit")]
        public ActionResult<FoodContainer> EditFoodContainer(FoodContainerEditDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            if (!AllowedName(request.Name))
                return BadRequest(_FoodContainer_Localizer["invalid.foodcontainername"]);

            FoodContainer foodContainer = _context.FoodContainers.FirstOrDefault(x => x.Id == request.FoodContainerID);

            if (foodContainer == null)
                return NotFound();

            if (_context.FoodContainers.Include(x => x.group).Any(x => x.Name.ToLower() == request.Name.ToLower() && x.group == foodContainer.group))
                return Conflict();


            //  bool updateFoodContainer = !(request.Name == string.Empty && request.Description == string.Empty);

            if (/*request.Name != string.Empty &&*/ request.Name != null)
                foodContainer.Name = request.Name;

            if (/*request.Description != string.Empty && */request.Description != null)
                foodContainer.Description = request.Description;

            /*if (updateFoodContainer)
            {*/
            foodContainer.UpdatedDate = DateTime.Now;
            _context.FoodContainers.Update(foodContainer);
            _context.SaveChanges();

            //}
            return Ok((FoodContainerPrivateDTO)foodContainer);
        }

        [HttpDelete("delete/{foodContainerID}")]
        public ActionResult<FoodContainer> DeleteFoodContainer(int foodContainerID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            FoodContainer foodContainer = _context.FoodContainers.FirstOrDefault(x => x.Id == foodContainerID);

            if (foodContainer == null)
                return NotFound();

            foodContainer.Empty(_context);
            _context.FoodContainers.Remove(foodContainer);
            _context.SaveChanges();

            return Ok((FoodContainerPrivateDTO)foodContainer);

        }

        [HttpGet("get/{FoodContainerID}")]
        public ActionResult<FoodContainer> getFoodContainer(int FoodContainerID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            FoodContainer foodcontainer = _context.FoodContainers.Include(x => x.Products).FirstOrDefault(x => x.Id == FoodContainerID);

            if (foodcontainer == null)
                return NotFound("foodcontainer introuvable");

            // todo sécurité, vérifier si le user est dans le groupe

            return Ok((FoodContainerPrivateDTO)foodcontainer);


        }

        /*[HttpDelete("delete/products")]
        public ActionResult<FoodContainer> deleteFoodContainerProducts(int FoodContainerID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            FoodContainer foodcontainer = _context.FoodContainers.Include(x => x.Products).FirstOrDefault(x => x.Id == FoodContainerID);

            if (foodcontainer == null)
                return NotFound("foodcontainer introuvable");

            foodcontainer.Products = new List<Product>();
            _context.FoodContainers.Update(foodcontainer);
            _context.SaveChanges();

            return Ok((FoodContainerPrivateDTO)foodcontainer);


        }*/

    }
}
