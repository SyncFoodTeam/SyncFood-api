using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.DTO.Input
{
    public class UserRegisterDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


        // Permet de caster un User en USerRegisterDTO

        public static explicit operator UserRegisterDTO(User user)
        {
            return new UserRegisterDTO { UserName = user.UserName, Email = user.Email, Password = user.Password };
        }
    }
}
