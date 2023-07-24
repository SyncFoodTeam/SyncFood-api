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
using SyncFoodApi.Controllers.Groups.DTO.Input;
using static SyncFoodApi.Controllers.Users.UserUtils;

namespace SyncFoodApi.Controllers.Groups
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly SyncFoodContext _context;
        private readonly IConfiguration _configuration;

        public GroupsController(SyncFoodContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("create")]
        public ActionResult<Group> GroupCreate(GroupCreateDTO request)
        {
            var user = getLogguedUser(User, _context);
            //if (user != null)
            //{
            Group group = new Group
            {
                Name = request.Name,
                Description = request.Description,
                Budget = request.Budget,
                Owner = user
            };

            _context.Groups.Add(group);
            _context.SaveChanges();
            return Ok(group);
            //}

            //else
                //return Unauthorized();
        }

    }
}
