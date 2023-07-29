using Microsoft.AspNetCore.Mvc;
using SyncFoodApi.dbcontext;
using SyncFoodApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static SyncFoodApi.Controllers.Users.UserUtils;
using static SyncFoodApi.Controllers.SyncFoodUtils;
using SyncFoodApi.Controllers.Users.DTO;
using NuGet.Protocol;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;

namespace SyncFoodApi.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly SyncFoodContext _context;
        private readonly IConfiguration _configuration;
        // private readonly ILogger _logger;
        private readonly IStringLocalizer<UserController> _Localizer;
        public UserController(SyncFoodContext context, IConfiguration configuration, IStringLocalizer<UserController> localizer)
        {
            // context de base de donnée
            _context = context;

            // config de l'appSettings.json
            _configuration = configuration;

            // Logger
            // _logger = logguer;

            // Traduction
            _Localizer = localizer;
            _Localizer = localizer;
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

            if (!AllowedName(request.UserName))
                return BadRequest(_Localizer["invalid.username"]);

            if (!IsValidEmail(request.Email))
                return BadRequest(_Localizer["invalid.email"]);

            if (!IsPasswordValid(request.Password))
                return BadRequest(_Localizer["invalid.password"]);

            if (EmailAlreadyUsed)
                return Conflict(_Localizer["unavailable.email"]);


            if (registeredUser.Discriminator == string.Empty)
                return Conflict(_Localizer["unavailable.username"]);


            _context.Users.Add(registeredUser);
            _context.SaveChanges();

            UserPrivateDTO userPrivate = (UserPrivateDTO)registeredUser;
            // _logger.LogDebug($"New user registered : {userPrivate.ToJson()}");
            return Ok(userPrivate);
        }


        [HttpPost("login"), AllowAnonymous]
        public ActionResult<User> UserLogin(UserLoginDTO request)
        {

            User user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == request.Email.ToLower());

            // mail qui existe pas ou mauvais mdp
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized(_Localizer["login.incorrect"]);


            if (user.Token == null)
            {
                user.Token = generateToken(_configuration, user);
                _context.Users.Update(user);
                _context.SaveChanges();
            }

            UserPrivateDTO userPrivate = (UserPrivateDTO)user;
            //  _logger.LogDebug($"User login : {userPrivate.ToJson()}");
            return Ok(userPrivate);

        }

        [HttpGet("info/me")]
        public ActionResult<User> UserSelfInfo()
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            UserPrivateDTO userPrivate = (UserPrivateDTO)user;
            return Ok(userPrivate);

        }

        [HttpGet("info/id/{userID}")]
        public ActionResult<User> GetUserById(int userID)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            User requestedUser = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (requestedUser == null)
                return NotFound(_Localizer["user.notfound"]);

            UserPublicDTO userPublic = (UserPublicDTO)user;
            return Ok(userPublic);

        }

        [HttpGet("info/username/{userName}/{discriminator}")]
        public ActionResult<User> GetUserByNameDiscriminator(string userName, string discriminator)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            User requestedUser = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower() && x.Discriminator == discriminator);

            if (requestedUser == null)
                return NotFound(_Localizer["user.notfound"]);

            UserPublicDTO userPublic = (UserPublicDTO)requestedUser;
            return Ok(userPublic);

        }

        [HttpPatch("update/me")]
        public ActionResult<User> UserUpdateMe(UserUpdateDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();


            if (request.UserName != null && request.UserName.ToLower() != user.UserName.ToLower())
            {
                if (!AllowedName(request.UserName))
                    return BadRequest(_Localizer["invalid.username"]);

                string newDiscriminator = discriminatorGenerator(_context, request.UserName);

                if (newDiscriminator == null)
                    return Conflict(_Localizer["unavailable.username"]);

                user.UserName = request.UserName;
                user.Discriminator = newDiscriminator;

            }

            if (request.Email != null && request.Email.ToLower() != user.Email.ToLower())
            {
                if (!IsValidEmail(request.Email))
                    return BadRequest(_Localizer["invalid.email"]);

                if (_context.Users.Any(x => x.Email.ToLower() == request.Email.ToLower()))
                    return Conflict(_Localizer["unavailable.email"]);

                user.Email = request.Email;

            }

            if (request.Password != null && !BCrypt.Net.BCrypt.Verify(request.Password,user.Password))
            {
                if (!IsPasswordValid(request.Password))
                    return BadRequest(_Localizer["invalid.password"]);

                user.Password = user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                // Par mesure de sécurité on génère un nouveau token quand l'utilisateur change son mot de passe
                // Ne sert à prioris à rien car va renvoyer le même token identique si l'ancien n'a pas expiré
                /*user.Token = UserUtils.generateToken(_configuration, user);*/

            }

            bool updateUser = request.UserName != null || request.Email != null || request.Password != null;

            if (updateUser)
            {
                user.UpdatedDate = DateTime.Now;
                _context.Users.Update(user);
                _context.SaveChanges();
            }

            UserPrivateDTO userPrivate = (UserPrivateDTO)user;
            return Ok(userPrivate);

        }


        [HttpDelete("delete/me")]
        public ActionResult<User> UserDeleteMe()
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            List<Group> userGroup = _context.Groups.Include(x => x.Members).Include(x => x.Owner).Where(x => x.Members.Contains(user)).ToList();

            foreach (Group group in userGroup)
            {
                // si il y'a plus d'un user on change le owner pour le prochain sur la liste
                // si non on supprime carrément le groupe
                if (group.Members.Count > 1)
                {
                    group.Members.Remove(user);
                    // group.Owner = null;

                    if (group.Owner == user)
                    {
                        group.Owner = group.Members[0];
                    }
                    group.UpdatedDate = DateTime.Now;
                }

                else
                {
                    _context.Groups.Remove(group);
                }

                _context.SaveChanges();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            UserPrivateDTO userPrivate = (UserPrivateDTO)user;
            return Ok(userPrivate);


        }

    }
}
