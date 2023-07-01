using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.dbcontext;
using SyncFoodApi.Models;
using BCrypt.Net;
using NuGet.Common;
using SyncFoodApi.Controllers.Users.DTO.Input;
using SyncFoodApi.Controllers.DTO.Input;
using SyncFoodApi.Controllers.DTO.Output;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol;
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

            if (EmailAlreadyUsed || !IsValidEmail(request.Email) || !IsPasswordValid(request.Password))
                return BadRequest();


            if (registeredUser.Discriminator == string.Empty)
                return Conflict("all discriminators are already taken for this username");


            _context.Users.Add(registeredUser);
            _context.SaveChanges();

            UserPrivateDTO userPrivateDTO = (UserPrivateDTO)registeredUser;

            return Ok(userPrivateDTO);
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

                UserPrivateDTO userCredential = (UserPrivateDTO)user;
                return Ok(userCredential);

            }

            else
            {
                return Unauthorized("The given email or password are incorrect");
            }
        }

        [HttpGet("info/me")]
        public ActionResult<User> UserSelfInfo()
        {
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            User user = _context.Users.FirstOrDefault(x => x.Email == userEmail);
            if (user != null)
            {
                UserPrivateDTO userCrendtial = (UserPrivateDTO)user;
                return Ok(userCrendtial);

            }

            else
                return NotFound();
        }

        [HttpGet("info/{userID}")]
        public ActionResult<User> UserInfo(int userID)
        {

            User user = _context.Users.FirstOrDefault(x => x.Id == userID);
            if (user != null)
            {
                UserPublicDTO userInfo = (UserPublicDTO)user;
                return Ok(userInfo);
            }

            else
            {
                return NotFound("There is no user corresponding to this id");
            }
        }

        [HttpPatch("update/me")]
        public ActionResult<User> UserUpdateMe(UserUpdateDTO request)
        {
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            User user = _context.Users.FirstOrDefault(x => x.Email == userEmail);

            if (user != null)
            {
                bool emailValid = false;
                bool passwordValid = false;
                bool updateUser = false;

                if (request.Email != null)
                {
                    emailValid = !_context.Users.Any(x => x.Email == request.Email) && IsValidEmail(request.Email);
                }

                if (request.Password != null)
                {
                    passwordValid = IsPasswordValid(request.Password);
                }

                if (request.Email != null || request.Password != null)
                {
                    updateUser = emailValid || passwordValid;
                }

                if (updateUser)
                {
                    if (emailValid)
                        user.Email = request.Email;

                    if (passwordValid)
                        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

                    _context.Users.Update(user);
                    _context.SaveChanges();

                    UserPrivateDTO userPrivate = (UserPrivateDTO)user;
                    return Ok(userPrivate);
                }

                else
                    return BadRequest();


            }

            else
                return NotFound();
            // si l'utilisateur autherntifié n'existe pas ou si aucune des valeurs email / mot de passe n'est renseigné on renvoi une badRequest
           /* if (user != null && (request.Email != null || request.Password != null))
            {
                // si l'email n'est pas déjà utilisé et/ou que le nouveau mot de passe est valide alors on met à jour
                bool emailValid= false;
                bool passwordValid = false;

                if (request.Email != null)
                    emailValid = !_context.Users.Any(x => x.Email == request.Email) && IsValidEmail(request.Email);
                
                if (request.Password != null)
                        passwordValid = IsPasswordValid(request.Password);

                bool updateUser = (request.Email != null && emailValid);

                if (updateUser)
                {

                    user.Email = request.Email;
                    user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    UserPrivateDTO userPrivate = (UserPrivateDTO)user;
                    return Ok(userPrivate);
                }

                else
                    return BadRequest();
            }

            return BadRequest();*/
        }


        [HttpDelete("delete/me")]
        public ActionResult<User> UserDeleteMe()
        {
            string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            User user = _context.Users.FirstOrDefault(x => x.Email == userEmail);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                UserPrivateDTO userPrivate = (UserPrivateDTO)user;
                return Ok(user);
            }

            return NotFound();

        }

    }
}
