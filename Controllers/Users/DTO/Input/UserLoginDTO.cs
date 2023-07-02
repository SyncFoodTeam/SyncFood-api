using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Users.DTO.Input
{
    public class UserLoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }


        // Permet de caster un User en USerLoginDTO

        public static explicit operator UserLoginDTO(User user)
        {
            return new UserLoginDTO { Email = user.Email, Password = user.Password };
        }
    }
}
