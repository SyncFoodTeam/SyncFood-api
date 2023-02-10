using Microsoft.AspNetCore.Mvc;
using SyncFoodApi.Controllers.DTO.Input;
using SyncFoodApi.Controllers.DTO.Output;
using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public UserController() 
        {
        }

        // Fonctions et Méthodes

        private string discriminatorGenerator()
        {
            Random random = new Random();
            string discriminator =  random.Next(1, 10000).ToString("D4");

            return discriminator;
        }



        // ROUTES
        [HttpGet("test")] public ActionResult<UserRegisterDTO> testUsers()
        {
            User userTest= new User();
            userTest.Email = "Jeanbon@mail";
            userTest.Discriminator = "8492";
            userTest.UserName = "Jeanbon";

            UserRegisterDTO userTestDTO = (UserRegisterDTO)userTest;
            
            return userTestDTO;
        }

        [HttpPost("register")] public ActionResult<User> UserRegister(UserRegisterDTO request) 
        {

            User user = new User();
            user.UserName = request.UserName;
            user.Email= request.Email;
            user.Discriminator = discriminatorGenerator();
            // mot de pass
            user.CreationDate = DateTime.Now;
            user.UpdatedDate = DateTime.Now;
            
            UserPrivateDTO userPrivateDTO= (UserPrivateDTO)user;

            return Ok(userPrivateDTO);
        }
    }
}
