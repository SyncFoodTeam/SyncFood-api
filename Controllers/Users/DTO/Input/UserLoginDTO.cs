using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Users.DTO.Input
{
    public class UserLoginDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


        // Permet de caster un User en USerLoginDTO

        public static explicit operator UserLoginDTO(User user)
        {
            return new UserLoginDTO { Email = user.Email, Password = user.Password };
        }
    }
}
