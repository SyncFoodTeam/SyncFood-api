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
        /*        private readonly ILogger _logger;
                private readonly IStringLocalizer _Localizer;*/
        public UserController(SyncFoodContext context, IConfiguration configuration)
        {
            // context de base de donnée
            _context = context;

            // config de l'appSettings.json
            _configuration = configuration;

            // Logger
            // _logger = logguer;

            // Traduction
            //  _Localizer = localizer;

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
                return BadRequest("Mail or password are invalid");


            if (registeredUser.Discriminator == string.Empty)
                return Conflict("This username is unavailable");


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

            // mail qui existe pas
            if (user == null)
                return Unauthorized("Incorrect mail or password");


            // mauvais mdp
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Incorrect mail or password");


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
                return NotFound();

            UserPublicDTO userPublic = (UserPublicDTO)user;
            return Ok(userPublic);

        }

        [HttpGet("info/username/{userNameDiscriminator}")]
        public ActionResult<User> GetUserByNameDiscriminator(string userNameDiscriminator)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            String[] splitted = userNameDiscriminator.Split('#');
            if (splitted.Length != 2)
                return BadRequest("Invalid username");

            String userName = splitted[0];
            String discriminator = splitted[1];
            User requestedUser = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower() && x.Discriminator == discriminator);

            if (requestedUser == null)
                return NotFound();

            UserPublicDTO userPublic = (UserPublicDTO)requestedUser;
            return Ok(userPublic);

        }

        [HttpPatch("update/me")]
        public ActionResult<User> UserUpdateMe(UserUpdateDTO request)
        {
            var user = getLogguedUser(User, _context);

            if (user == null)
                return Unauthorized();

            bool emailValid = false;
            bool passwordValid = false;
            bool userNameValid = false;

            bool emailAsChanged = false;
            bool passwordAsChanged = false;
            bool userNameUpdated = false;

            bool updateUser = false;

            string newDiscriminator = string.Empty;

            if (request.UserName != null)
            {
                userNameUpdated = true;

                if (!AllowedName(request.UserName))
                    return BadRequest("Invalid username");

                newDiscriminator = discriminatorGenerator(_context, request.UserName);

                if (newDiscriminator == null)
                    return Conflict("Username unavailable");

                userNameValid = true;
                user.UserName = request.UserName;

            }

            if (request.Email != null)
            {
                emailAsChanged = true;
                emailValid = !_context.Users.Any(x => x.Email == request.Email) && IsValidEmail(request.Email);

                if (emailValid)
                    user.Email = request.Email;
            }

            if (request.Password != null)
            {
                passwordAsChanged = true;
                passwordValid = IsPasswordValid(request.Password);

                if (passwordValid)
                {
                    user.Password = user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    // Par mesure de sécurité on génère un nouveau token quand l'utilisateur change son mot de passe
                    user.Token = UserUtils.generateToken(_configuration, user);

                }


            }

            // Détermine si on doit mettre à jours l'utilisateur
            // pour se faire le mail le mot de passe et le nom d'utilisateur doivent être valide si ils sont renseigné
            updateUser = !(emailAsChanged && !emailValid || passwordAsChanged && !passwordValid || userNameUpdated && !userNameValid);


            if (updateUser)
            {

                user.UpdatedDate = DateTime.Now;

                _context.Users.Update(user);
                _context.SaveChanges();


            }

            UserPrivateDTO userPrivate = (UserPrivateDTO)user;
            return Ok(userPrivate);
            /*            else
                            return BadRequest();*/

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
