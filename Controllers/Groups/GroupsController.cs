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
using static SyncFoodApi.Controllers.Groups.GroupUtils;
using NuGet.Protocol;

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
            if (_context.Groups.Include(group => group.Owner).Any(x => x.Name.ToLower() == request.Name.ToLower() && x.Owner == user))
            {
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

            group.Members.Add(user);

            _context.Groups.Add(group);
            _context.SaveChanges();
            return Ok((GroupPrivateDTO)group);
            //}

            //else
            //return Unauthorized();
        }

        [HttpPatch("edit")]
        public ActionResult<Group> GroupEdit(GroupEditDTO request)
        {
            var user = getLogguedUser(User, _context);
            var group = _context.Groups.Include(x => x.Owner).FirstOrDefault(x => x.Id == request.groupID);

            if (group != null)
            {
                if (group.Owner.Id == user.Id)
                {
                    if (request.Name != string.Empty && request.Name != null)
                        group.Name = request.Name;

                    if (request.Description != string.Empty && request.Description != null)
                        group.Description = request.Description;

                    if (request.Budget != 0f)
                        group.Budget = request.Budget;

                    group.UpdatedDate = DateTime.Now;
                    _context.Groups.Update(group);
                    _context.SaveChanges();

                    return Ok((GroupPrivateDTO)group);

                }
                else
                    return Unauthorized();
            }

            else
                return NotFound();

        }

        [HttpGet("mine")]
        public ActionResult<List<GroupPrivateDTO>> GroupGetMine()
        {
            var user = getLogguedUser(User, _context);

            List<GroupPrivateDTO> publicGroups = new List<GroupPrivateDTO>();
            var groups = _context.Groups.Include(group => group.Owner).Include(x => x.Members).Where(x => x.Members.Contains(user)).ToList();


                foreach (Group group in groups)
                {
                    publicGroups.Add((GroupPrivateDTO)group);
                }
                return Ok(publicGroups);


        }

        [HttpDelete("delete/{groupid}")]
        public ActionResult<GroupPrivateDTO> GroupDelete(int groupID)
        {
            var user = getLogguedUser(User, _context);
            Group group = _context.Groups.Include(x => x.Owner).Include(x => x.Members).FirstOrDefault(x => x.Id == groupID);

            if (group != null)
            {
                if (group.Owner.Id == user.Id)
                {
                    _context.Groups.Remove(group);
                    _context.SaveChanges();
                    return Ok((GroupPrivateDTO)group);
                }

                else
                    return Forbid();
            }

            else
                return NotFound();
        }


        [HttpPatch("members/add")]
        public ActionResult<GroupPrivateDTO> GroupMembersAdd(int groupID, int userID)
        {
            var user = getLogguedUser(User, _context);
            Group group = _context.Groups.Include(x => x.Owner).Include(x => x.Members).FirstOrDefault(x => x.Id == groupID);
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
                    return Forbid();
            }

            else
                return NotFound();
        }

        [HttpPatch("members/remove")]
        public ActionResult<GroupPrivateDTO> GroupMembersRemove(int groupID, int userID)
        {
            var user = getLogguedUser(User, _context);
            Group group = _context.Groups.Include(x => x.Owner).Include(x => x.Members).FirstOrDefault(x => x.Id == groupID);
            User userToRemove = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (group != null && userToRemove != null)
            {
                if (group.Owner.Id == user.Id)
                {
                    if (group.Members.Contains(userToRemove) && userID != group.Owner.Id)
                    {

                        group.Members.Remove(userToRemove);
                        _context.SaveChanges();
                        return Ok((GroupPrivateDTO)group);
                    }

                    else
                        return BadRequest();
                }

                else
                    return Forbid();
            }

            else
                return NotFound();
        }

        [HttpPatch("changeOwner")]
        public ActionResult<UserPublicDTO> GroupChangeOwner(int groupID, int userID)
        {
            var user = getLogguedUser(User, _context);
            Group group = _context.Groups.Include(x => x.Owner).Include(x => x.Members).FirstOrDefault(x => x.Id == groupID);
            User newOwner = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (user != null && newOwner != null && group != null)
            {
                if (user.Id == group.Owner.Id)
                {
                    if (group.Members.Contains(newOwner))
                    {

                        group.Owner = newOwner;
                        _context.Groups.Update(group);
                        _context.SaveChanges();
                        return Ok((GroupPrivateDTO)group);
                    }

                    else
                        return BadRequest();

                }

                else
                    return Forbid();
            }

            else
                return NotFound();

        }

    }
}
