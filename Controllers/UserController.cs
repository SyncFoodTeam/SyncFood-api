using Microsoft.AspNetCore.Mvc;
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

        [HttpGet] public ActionResult<User> getUsers()
        {
            User userTest= new User();
            userTest.Email = "Jeanbon@mail";
            userTest.Discriminator = "8492";
            userTest.Name = "Jeanbon";
            
            return userTest;
        }
    }
}
