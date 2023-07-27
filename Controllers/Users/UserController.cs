using Microsoft.AspNetCore.Mvc;
using SyncFoodApi.dbcontext;
using SyncFoodApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static SyncFoodApi.Controllers.Users.UserUtils;
using SyncFoodApi.Controllers.Users.DTO;
using NuGet.Protocol;

namespace SyncFoodApi.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly SyncFoodContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public UserController(SyncFoodContext context, IConfiguration configuration, ILogger<UserController> logguer) 
        {
            // context de base de donnée
            _context = context;
            // config de l'appSettings.json
            _configuration = configuration;

            // Logger
            _logger = logguer;
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

            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
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

                else
                    return Unauthorized("Incorrect mail or password");

            }

            else
                return Unauthorized("Incorrect mail or password");
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

        [HttpGet("info/id/{userID}")]
        public ActionResult<User> UserInfo(int userID)
        {

            User user = _context.Users.FirstOrDefault(x => x.Id == userID);
            if (user != null)
            {
                UserPublicDTO userPublic = (UserPublicDTO)user;
                return Ok(userPublic);
            }

            else
                return NotFound();
        }

        [HttpGet("info/username/{userNameDiscriminator}")]
        public ActionResult<User> GetUserByNameDiscriminator(string userNameDiscriminator)
        {
            String[] splitted = userNameDiscriminator.Split('#');
            if (splitted.Length == 2)
            {
                String userName = splitted[0];
                String discriminator = splitted[1];
                User user = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == userName.ToLower() && x.Discriminator == discriminator);

                if (user != null)
                {
                    return Ok((UserPublicDTO)user);
                }

                else
                    return NotFound();
            }

            else
                return BadRequest("Invalid username");
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

                bool emailAsChanged = false;
                bool passwordAsChanged = false;
                bool userNameValid = false;

                bool updateUser = false;

                string newUserName = string.Empty;
                string newDiscriminator = string.Empty;

                if (request.UserName != null)
                {

                    userNameUpdated= true;
                    newUserName = request.UserName;
                    if (IsUserNameValide(newUserName))
                    {

                        newDiscriminator = discriminatorGenerator(_context, request.UserName);
                        if (newDiscriminator != null)
                        {
                            userNameValid = true;
                        }

                        else
                            return Conflict("Username unavailable");
                    }

                    else
                        return BadRequest("Invalid username");

                }

                if (request.Email != null)
                {
                    emailAsChanged = true;
                    emailValid = !_context.Users.Any(x => x.Email == request.Email) && IsValidEmail(request.Email);
                }

                if (request.Password != null)
                {
                    passwordAsChanged = true;
                    passwordValid = IsPasswordValid(request.Password);
                }

                // Détermine si on doit mettre à jours l'utilisateur
                // pour se faire le mail le mot de passe et le nom d'utlisateur doivent être valide si ils sont renseigné
                updateUser = !( emailAsChanged && !emailValid || passwordAsChanged && !passwordValid || userNameUpdated && !userNameValid );


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
