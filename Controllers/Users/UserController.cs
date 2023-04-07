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

        // Fonctions et Méthodes


        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private bool IsPasswordValid(string password)
        {
            if (password.Length < 6)
            {
                return false;
            }

            bool containLower = false;
            bool containUpper = false;

            foreach (char c in password)
            {
                if (char.IsLower(c)) containLower = true;
                if (char.IsUpper(c)) containUpper = true;
            }

            return (containLower && containUpper);
        }

        private string discriminatorGenerator(string userName)
        {
            Random random = new Random();
            string discriminator = string.Empty;
            int retry = 0;
            do
            {
                discriminator = random.Next(1, 10000).ToString("D4"); // retourne une string de 4 digits exemple 5 en int => 0005 en string
                retry++;

            } while (isUserNameDiscriminatorExist(userName, discriminator) && retry < 10000); // On continue de générer un nouveau discriminant tant que celui généré actuellement existe déjà et que le nombre d'essais est inférieur au nombre total de possibilité (pour éviter une boucle infini dans le cas extrême où toute les combinaisons seraient déjà prises

            if (_context.Users.Any(x => x.Discriminator == discriminator))
            {
                return string.Empty;
            }

            return discriminator;
        }

        // Détermine si un combo pseudo+discriminant existe déjà (exemple : monSuperPseudo#8392)
        private bool isUserNameDiscriminatorExist(string wantedUserName, string wantedDiscriminator)
        {

            var usersList = _context.Users.Where(x => x.UserName.ToLower() == wantedUserName.ToLower());
            bool exist = usersList.Any(x => x.Discriminator == wantedDiscriminator);
            return exist;
        }

        private string generateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims:claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        // ROUTES

        [HttpPost("register"), AllowAnonymous]
        public ActionResult<User> UserRegister(UserRegisterDTO request)
        {

            User registeredUser = new User();


            bool EmailAlreadyUsed = _context.Users.Any(x => x.Email.ToLower() == request.Email.ToLower());

            if (EmailAlreadyUsed)
                return Conflict("This email address is already used");

            if (!IsValidEmail(request.Email))
                return BadRequest("The request email is invalid");


            registeredUser.Email = request.Email;
            registeredUser.UserName = request.UserName;


            registeredUser.Discriminator = discriminatorGenerator(registeredUser.UserName);

            if (registeredUser.Discriminator == string.Empty)
                return Conflict("all discriminators are already taken for this username");

            if (!IsPasswordValid(request.Password))
                return BadRequest("The request password is invalid");

            registeredUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            registeredUser.CreationDate = DateTime.Now;
            registeredUser.UpdatedDate = DateTime.Now;


            // sauvegarde du nouveau user dans la db
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
                // TODO TOKEN GENERATION
                if (user.Token == string.Empty)
                {
                    user.Token = generateToken(user);
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

        [Authorize]
        [HttpGet("info/me")]
        public ActionResult<User> UserInfoSelf([FromHeader(Name = "token")] string token)
        {

            User user = _context.Users.FirstOrDefault(x => x.Token == token);
            UserPrivateDTO userCrendtial = (UserPrivateDTO)user;
            return Ok(userCrendtial);
        }

        [Authorize]
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

    }
}
