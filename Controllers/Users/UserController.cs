﻿using Microsoft.AspNetCore.Mvc;
using SyncFoodApi.dbcontext;
using SyncFoodApi.Models;
using SyncFoodApi.Controllers.Users.DTO.Input;
using SyncFoodApi.Controllers.DTO.Input;
using SyncFoodApi.Controllers.DTO.Output;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static SyncFoodApi.Controllers.Users.UserUtils;

namespace SyncFoodApi.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly SyncFoodContext _context;
        private readonly IConfiguration _configuration;
        public UserController(SyncFoodContext context, IConfiguration configuration)
        {
            // context de base de donnée
            _context = context;
            // config de l'appSettings.json
            _configuration = configuration;
        }



        // ROUTES

        [HttpPost("register"), AllowAnonymous]
        public ActionResult<User> UserRegister(UserRegisterDTO request)
        {

            User registeredUser = new User
            {
                Email = request.Email,
                UserName = request.UserName,
                Discriminator = discriminatorGenerator(_context, request.UserName),
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreationDate = DateTime.Now,
                UpdatedDate = DateTime.Now

            };

            bool EmailAlreadyUsed = _context.Users.Any(x => x.Email.ToLower() == request.Email.ToLower());

            // si l'email est déjà utilisé ou invalide ou encore si le mot de passe n'est pas valide on return
            if (EmailAlreadyUsed || !IsValidEmail(request.Email) || !IsPasswordValid(request.Password))
                return BadRequest();


            if (registeredUser.Discriminator == string.Empty)
                return Conflict("all discriminators are already taken for this username");


            _context.Users.Add(registeredUser);
            _context.SaveChanges();

            return Ok((UserPrivateDTO)registeredUser);
        }

        [HttpPost("login"), AllowAnonymous]
        public ActionResult<User> UserLogin(UserLoginDTO request)
        {

            User user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == request.Email.ToLower());

            if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {

                if (user.Token == null)
                {
                    user.Token = generateToken(_configuration, user);
                    _context.Users.Update(user);
                    _context.SaveChanges();
                }

                return Ok((UserPrivateDTO)user);

            }

            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("info/me")]
        public ActionResult<User> UserSelfInfo()
        {
            var user = getLogguedUser(User, _context);
            if (user != null)
                return Ok((UserPrivateDTO)user);
            else
                return Unauthorized();
        }

        [HttpGet("info/{userID}")]
        public ActionResult<User> UserInfo(int userID)
        {

            User user = _context.Users.FirstOrDefault(x => x.Id == userID);
            if (user != null)
            {
                UserPublicDTO userPublic = (UserPublicDTO)user;
                return Ok(userPublic);
            }

            else
                return NotFound("There is no user corresponding to this id");
        }

        [HttpPatch("update/me")]
        public ActionResult<User> UserUpdateMe(UserUpdateDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user != null)
            {
                bool emailValid = false;
                bool passwordValid = false;
                bool userNameUpdated = false;
                bool updateUser = false;

                string newUserName = string.Empty;
                string newDiscriminator = string.Empty;

                if (request.UserName != null)
                {
                    newUserName = request.UserName;
                    newDiscriminator = discriminatorGenerator(_context, request.UserName);
                    if (newDiscriminator != null)
                    {
                        userNameUpdated = true;
                    }

                    else
                        return Conflict();
                }

                if (request.Email != null)
                {
                    emailValid = !_context.Users.Any(x => x.Email == request.Email) && IsValidEmail(request.Email);
                }

                if (request.Password != null)
                {
                    passwordValid = IsPasswordValid(request.Password);
                }


                updateUser = emailValid || passwordValid || userNameUpdated;


                if (updateUser)
                {
                    if (emailValid)
                        user.Email = request.Email;

                    if (passwordValid)
                        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

                    if (userNameUpdated)
                    {
                        user.UserName = newUserName;
                        user.Discriminator = newDiscriminator;
                    }

                    user.UpdatedDate = DateTime.Now;

                    _context.Users.Update(user);
                    _context.SaveChanges();

                    return Ok((UserPrivateDTO)user);
                }

                else
                    return BadRequest();

            }

            else
                return Unauthorized();

        }


        [HttpDelete("delete/me")]
        public ActionResult<User> UserDeleteMe()
        {
            var user = getLogguedUser(User, _context);
            // manager
            if (user != null)
            {
                List<Group> userGroup = _context.Groups.Where(x => x.Members.Contains(user)).ToList();

                foreach (Group group in userGroup)
                {
                    // si il y'a plus d'un user on change le owner pour le prochain sur la liste
                    // si non on supprime carrément le groupe
                    if (group.Members.Count > 1)
                    {
                        group.Members.Remove(user);
                        group.Owner = null;

                        if (group.Owner.Id == user.Id)
                        {
                            group.Owner = group.Members[0];
                        }
                    }

                    else
                    {
                        _context.Groups.Remove(group);
                    }

                    group.UpdatedDate = DateTime.Now;

                    _context.SaveChanges();
                }

                _context.Users.Remove(user);
                _context.SaveChanges();

                return Ok((UserPrivateDTO)user);
            }

            else
                return Unauthorized();

        }

    }
}
