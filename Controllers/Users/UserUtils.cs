using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SyncFoodApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SyncFoodApi.dbcontext;
using System.Text.RegularExpressions;

namespace SyncFoodApi.Controllers.Users
{
    public static class UserUtils
    {
        // Fonctions et Méthodes

        public static bool IsUserNameValide(string userName)
        {
            Regex regex = new Regex("^[a-zA-Z0-9 ]*$");
            return regex.IsMatch(userName);

        }


        public static bool IsValidEmail(string email)
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

        /* 
         * Min 6 caractères
         * Contient des maj et min
         */

        public static bool IsPasswordValid(string password)
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

        public static string discriminatorGenerator(SyncFoodContext _context ,string userName)
        {
            Random random = new Random();
            string discriminator = string.Empty;
            int retry = 0;
            do
            {
                discriminator = random.Next(1, 10000).ToString("D4"); // retourne une string de 4 digits exemple 5 en int => 0005 en string
                retry++;

            } while (isUserNameDiscriminatorExist(_context, userName, discriminator) && retry < 10000); // On continue de générer un nouveau discriminant tant que celui généré actuellement existe déjà et que le nombre d'essais est inférieur au nombre total de possibilité (pour éviter une boucle infini dans le cas extrême où toute les combinaisons seraient déjà prises

            if (_context.Users.Any(x => x.Discriminator == discriminator))
            {
                return string.Empty;
            }

            return discriminator;
        }

        // Détermine si un combo pseudo+discriminant existe déjà (exemple : monSuperPseudo#8392)
        public static bool isUserNameDiscriminatorExist(SyncFoodContext _context , string wantedUserName, string wantedDiscriminator)
        {

            var usersList = _context.Users.Where(x => x.UserName.ToLower() == wantedUserName.ToLower());
            bool exist = usersList.Any(x => x.Discriminator == wantedDiscriminator);
            return exist;
        }

        public static string generateToken(IConfiguration _configuration ,User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public static User getLogguedUser(ClaimsPrincipal User, SyncFoodContext _context)
        {
            int userID = int.Parse( User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            User user = _context.Users.FirstOrDefault(x => x.Id == userID);

            if (user.Token != null && user.Token != string.Empty)
            {

                return user;
            }

            return null;

        }
    }
}
