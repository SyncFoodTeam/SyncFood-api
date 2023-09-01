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
using SyncFoodApi.Controllers.Groups.DTO;
using SyncFoodApi.Controllers.Users.DTO;
using static SyncFoodApi.Controllers.Users.UserUtils;
using static SyncFoodApi.Controllers.SyncFoodUtils;
using static SyncFoodApi.Controllers.Groups.GroupUtils;
using NuGet.Protocol;
using Microsoft.Extensions.Localization;
using SyncFoodApi.Controllers.Users;

namespace SyncFoodApi.Controllers.Groups
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly SyncFoodContext _context;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<GroupsController> _Group_localization;
        private readonly IStringLocalizer<UserController> _User_localization;

        public GroupsController(SyncFoodContext context, IConfiguration configuration, IStringLocalizer<GroupsController> GroupLocalizer, IStringLocalizer<UserController> UserLocalizer)
        {
            _context = context;
            _configuration = configuration;
            _Group_localization = GroupLocalizer;
            _User_localization = UserLocalizer;
        }

        [HttpPost("create")]
        public ActionResult<Group> GroupCreate(GroupCreateDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();


            if (!AllowedName(request.Name))
                return BadRequest(_Group_localization["invalid.groupname"]);

            // on vérifie que l'utilisateur n'a pas déjà un groupe du même nom
            if (_context.Groups.Include(group => group.Owner).Any(x => x.Name.ToLower() == request.Name.ToLower() && x.Owner == user))
                return Conflict(_Group_localization["group.alreadyexist"]);

            if (request.Budget < 0)
                return BadRequest(_Group_localization["invalid.budget"]);

            Group group = new Group
            {
                Name = request.Name,
                Description = request.Description,
                Budget = request.Budget,
                Owner = user
            };

            group.Members.Add(user);

            _context.Groups.Add(group);
            _context.SaveChanges();

            GroupPrivateLitedDTO groupPrivateLite = (GroupPrivateLitedDTO)group;
            return Ok(groupPrivateLite);

        }

        [HttpPatch("edit")]
        public ActionResult<Group> GroupEdit(GroupEditDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
            {
                return Unauthorized();
            }

            Group group = _context.Groups.Include(x => x.Owner).FirstOrDefault(x => x.Id == request.groupID);

            if (group == null)
                return NotFound(_Group_localization["group.notfound"]);

            if (group.Owner != user)
                return Forbid(_Group_localization["group.notowner"]);

            bool nameUpdated = false;
            bool descriptionUpdated = false;
            bool budgetUpdated = false;

            if (request.Name != null && request.Name.ToLower() != group.Name.ToLower())
            {
                if (!AllowedName(request.Name))
                    return BadRequest(_Group_localization["invalid.groupname"]);

                group.Name = request.Name;
                nameUpdated = true;
            }

            if (request.Description != null && request.Description.ToLower() != group.Description.ToLower())
            {
                group.Description = request.Description;
                descriptionUpdated = true;

            }

            if (request.Budget != group.Budget)
            {

                if (request.Budget < 0)
                    return BadRequest(_Group_localization["invalid.budget"]);

                group.Budget = request.Budget;
                budgetUpdated = true;
            }

            bool updateGroup = nameUpdated || descriptionUpdated || budgetUpdated;

            if (updateGroup)
            {
                group.UpdatedDate = DateTime.Now;
                _context.Groups.Update(group);
                _context.SaveChanges();

            }

            GroupPrivateLitedDTO groupPrivateLite = (GroupPrivateLitedDTO)group;
            return Ok(groupPrivateLite);

        }

        [HttpGet("mine")]
        public ActionResult<List<GroupPrivateDTO>> GroupGetMine()
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            var groups = _context.Groups.Include(group => group.Owner).Where(x => x.Members.Contains(user)).ToList();
          
            return Ok(GetGroupsPrivateliteDTO(groups));

        }

        [HttpGet("get/{groupID}")]
        public ActionResult<Group> getGroup(int groupID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            var group = _context.Groups.Include(x => x.Members).Include(x => x.FoodContainers).Include(x => x.ShoppingList).Include(x => x.Owner).FirstOrDefault(x => x.Id == groupID);

            if (group == null)
                return NotFound(_Group_localization["group.notfound"]);

            if (!group.Members.Contains(user))
                return Forbid(_Group_localization["group.noaccess"]);

            GroupPrivateDTO groupPrivate = (GroupPrivateDTO)group;
            return Ok(groupPrivate);

        }

        [HttpDelete("delete/{groupID}")]
        public ActionResult<GroupPrivateDTO> GroupDelete(int groupID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            Group group = _context.Groups.Include(x => x.Owner).Include(x => x.Members).FirstOrDefault(x => x.Id == groupID);

            if (group == null)
                return NotFound(_Group_localization["group.notfound"]);

            if (group.Owner != user)
                return Forbid(_Group_localization["group.notowner"]);

            _context.Groups.Remove(group);
            _context.SaveChanges();

            GroupPrivateLitedDTO groupPrivateLite = (GroupPrivateLitedDTO)group;
            return Ok(groupPrivateLite);

        }


        [HttpPatch("members/add/{groupID}/{userID}")]
        public ActionResult<GroupPrivateDTO> GroupMembersAdd(int groupID, int userID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            Group group = _context.Groups.Include(x => x.Owner).Include(x => x.Members).FirstOrDefault(x => x.Id == groupID);
            User userToAdd = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (group == null)
                return NotFound(_Group_localization["group.notfound"]);

            if (userToAdd == null)
                return NotFound(_User_localization["user.notfound"]);

            if (group.Owner != user)
                return Forbid(_Group_localization["group.notowner"]);


            if (!group.Members.Contains(userToAdd))
            {
                group.Members.Add(userToAdd);
                _context.Groups.Update(group);
                _context.SaveChanges();
            }

            GroupPrivateDTO groupPrivate = (GroupPrivateDTO)group;
            return Ok(groupPrivate);



        }

        [HttpPatch("members/remove/{groupID}/{userID}")]
        public ActionResult<GroupPrivateDTO> GroupMembersRemove(int groupID, int userID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            Group group = _context.Groups.Include(x => x.Owner).Include(x => x.Members).FirstOrDefault(x => x.Id == groupID);
            User userToRemove = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (group == null)
                return NotFound(_Group_localization["group.notfound"]);

            if (userToRemove == null)
                return NotFound(_User_localization["user.notfound"]);

            if (group.Owner != user)
                return Forbid(_Group_localization["group.notowner"]);


            if (group.Members.Contains(userToRemove))
            {
                group.Members.Remove(userToRemove);
                _context.Groups.Update(group);
                _context.SaveChanges();
            }

            GroupPrivateDTO groupPrivate = (GroupPrivateDTO)group;
            return Ok(groupPrivate);
        }

        [HttpPatch("changeOwner/{groupID}/{userID}")]
        public ActionResult<UserPublicDTO> GroupChangeOwner(int groupID, int userID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            Group group = _context.Groups.Include(x => x.Owner).Include(x => x.Members).FirstOrDefault(x => x.Id == groupID);
            User newOwner = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (group == null)
                return NotFound(_Group_localization["group.notfound"]);

            if (newOwner == null)
                return NotFound(_User_localization["user.notfound"]);

            if (user != group.Owner)
                return Forbid(_Group_localization["group.notowner"]);

            if (!group.Members.Contains(newOwner))
                return BadRequest(_Group_localization["group.usernotin"]);

            group.Owner = newOwner;
            _context.Groups.Update(group);
            _context.SaveChanges();

            GroupPrivateDTO groupPrivate = (GroupPrivateDTO)group;
            return Ok(groupPrivate);

        }

    }
}
