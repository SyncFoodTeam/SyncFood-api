﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncFoodApi.Controllers.DTO.Input;
using SyncFoodApi.Controllers.DTO.Output;
using SyncFoodApi.dbcontext;
using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
            private readonly SyncFoodContext _context;
        public UserController(SyncFoodContext context) 
        {
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
                discriminator = random.Next(1, 10000).ToString("D4");
                retry++;

            } while (isUserNameDiscriminatorExist(userName,discriminator) && retry < 10000); // On continue de générer un nouveau discriminant tant que celui généré actuellement existe déjà et que le nombre d'essais est inférieur au nombre total de possibilité (pour éviter une boucle infini dans le cas extrême où toute les combinaisons seraient déjà prises

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
            var exist = usersList.Any(x => x.Discriminator == wantedDiscriminator);
            return exist;
        }


        // ROUTES
        [HttpGet("test")] public ActionResult<UserRegisterDTO> testUsers()
        {

            User userTest= new User();
            userTest.Email = "Jeanbon@mail";
            userTest.Discriminator = "8492";
            userTest.UserName = "Jeanbon";


            // vérification des données

            UserRegisterDTO userTestDTO = (UserRegisterDTO)userTest;
            
            return Ok(userTestDTO);
        }

        [HttpPost("register")] public ActionResult<User> UserRegister(UserRegisterDTO request) 
        {
      
            
            User registeredUser = new User();


            /*var EmailAlreadyUsed = _context.Users.Any(x => x.Email.ToLower() == request.Email.ToLower());

            if (EmailAlreadyUsed)
            {
                return Conflict("This email address is already used");
            }*/

            registeredUser.Email= request.Email;
            registeredUser.UserName = request.UserName;


            registeredUser.Discriminator = discriminatorGenerator(registeredUser.UserName);

            if (registeredUser.Discriminator == string.Empty)
            {
                return Conflict("all discriminators are already taken for this username");
            }


            // mot de pass hashé


            registeredUser.CreationDate = DateTime.Now;
            registeredUser.UpdatedDate = DateTime.Now;


            // sauvegarde du nouveau user dans la db
            _context.Users.Add(registeredUser);
            _context.SaveChanges();

            UserPrivateDTO userPrivateDTO= (UserPrivateDTO)registeredUser;

            return Ok(userPrivateDTO);
        }
    }
}
