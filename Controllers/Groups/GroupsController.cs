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
using SyncFoodApi.Controllers.Groups.DTO.Output;

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
            if (_context.Groups.Any(x => x.Name.ToLower() == request.Name.ToLower() && x.Owner == user)){
                return Conflict();
            }
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
            return Ok((GroupPrivateDTO)group);
            //}

            //else
                //return Unauthorized();
        }

        [HttpPatch("edit")]
        public ActionResult<Group> GroupEdit(GroupEditDTO request,int groupID)
        {
            var user = getLogguedUser(User, _context);
            Group group = _context.Groups.FirstOrDefault(x => x.Id == groupID);

            if (group != null)
            {
                if (group.Owner == user)
                {
                    if (request.Name != string.Empty)
                        group.Name = request.Name;
                    if (request.Description != string.Empty)
                        group.Description = request.Description;
                    if (request.Budget != 0f)
                        group.Budget = request.Budget;

                    _context.Groups.Add(group);
                    _context.SaveChanges();

                    return Ok((GroupPrivateDTO)group);

                }
                else
                    return Unauthorized();
            }

            else
                return NotFound();

        }

        [HttpDelete("delete")]
        public ActionResult<GroupPrivateDTO> GroupDelete(int groupID)
        {
            var user = getLogguedUser(User, _context);
            Group group = _context.Groups.FirstOrDefault(x => x.Id == groupID);

            if (group != null)
            {
                if (group.Owner == user)
                {
                    _context.Groups.Remove(group);
                    _context.SaveChanges();
                    return Ok((GroupPrivateDTO)group);
                }

                else
                    return Unauthorized();
            }

            else
                return NotFound();
        }

        [HttpPatch("members/add")]
        public ActionResult<GroupPrivateDTO> GroupMembersAdd(int groupID, int userID)
        {
            var user = getLogguedUser(User, _context);
            Group group = _context.Groups.FirstOrDefault(x => x.Id == groupID);
            User userToAdd = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (group != null && userToAdd != null)
            {
                if (group.Owner == user)
                {
                    if (!group.Members.Contains(userToAdd))
                    {
                        group.Members.Add(userToAdd);
                        _context.SaveChanges();
                        return Ok((GroupPrivateDTO)group);
                    }

                    else
                        return BadRequest();
                }

                else
                    return Unauthorized();
            }

            else
                return NotFound();
        }

        [HttpPatch("members/remove")]
        public ActionResult<GroupPrivateDTO> GroupMembersRemove(int groupID, int userID)
        {
            var user = getLogguedUser(User, _context);
            Group group = _context.Groups.FirstOrDefault(x => x.Id == groupID);
            User userToRemove = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (group != null && userToRemove != null)
            {
                if (group.Owner == user)
                {
                    if (group.Members.Contains(userToRemove)){

                        group.Members.Remove(userToRemove);
                        _context.SaveChanges();
                        return Ok((GroupPrivateDTO)group);
                    }

                    else
                        return BadRequest();
                }

                else
                    return Unauthorized();
            }

            else
                return NotFound();
        }

    }
}
