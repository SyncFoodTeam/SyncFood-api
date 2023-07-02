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


        /*[HttpPost("create")]
        public ActionResult<Group> GroupCreate()
        {

        }*/

    }
}
