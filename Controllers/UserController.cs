using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.Controllers.DTO.Input;
using SyncFoodApi.Controllers.DTO.Output;
using SyncFoodApi.dbcontext;
using SyncFoodApi.Models;
using BCrypt.Net;
using NuGet.Common;

namespace SyncFoodApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly SyncFoodContext _context;
        public UserController(SyncFoodContext context)
        {
            // context de base de donnée
            _context = context;
        }

        // Fonctions et Méthodes

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

        // Extrêmement basique mais fait le taff pour l'instant
        private string generateToken()
        {
            var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var resultToken = new string(
               Enumerable.Repeat(allChar, 70)
               .Select(token => token[random.Next(token.Length)]).ToArray());

            string authToken = resultToken.ToString();
            return authToken;
        }

        private Boolean isTokenValid(string token)
        {
           if ( (_context.Users.FirstOrDefault(x => x.Token == token)) != null)
            {
                return true;
            }

           return false;
        }


        // ROUTES

        [HttpPost("register")]
        public ActionResult<User> UserRegister(UserRegisterDTO request)
        {

            User registeredUser = new User();


            bool EmailAlreadyUsed = _context.Users.Any(x => x.Email.ToLower() == request.Email.ToLower());

            if (EmailAlreadyUsed)
            {
                return Conflict("This email address is already used");
            }

            registeredUser.Email = request.Email;
            registeredUser.UserName = request.UserName;


            registeredUser.Discriminator = discriminatorGenerator(registeredUser.UserName);

            if (registeredUser.Discriminator == string.Empty)
            {
                return Conflict("all discriminators are already taken for this username");
            }


            registeredUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            registeredUser.CreationDate = DateTime.Now;
            registeredUser.UpdatedDate = DateTime.Now;


            // sauvegarde du nouveau user dans la db
            _context.Users.Add(registeredUser);
            _context.SaveChanges();

            UserPrivateDTO userPrivateDTO = (UserPrivateDTO)registeredUser;

            return Ok(userPrivateDTO);
        }

        [HttpPost("login")]
        public ActionResult<User> UserLogin(UserLoginDTO request)
        {

            User user = _context.Users.FirstOrDefault(x => x.Email.ToLower() == request.Email.ToLower());

            if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                // TODO TOKEN GENERATION
                if (user.Token == string.Empty)
                {
                    user.Token = generateToken();
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
        public ActionResult<User> UserInfoSelf([FromHeader(Name ="token")] string token)
        {
            if (!isTokenValid(token))
            {
                return Unauthorized("The given token is invalid");
            }

            User user = _context.Users.FirstOrDefault(x =>x.Token == token);
            UserPrivateDTO userCrendtial = (UserPrivateDTO)user;
            return Ok(userCrendtial);
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

    }
}
