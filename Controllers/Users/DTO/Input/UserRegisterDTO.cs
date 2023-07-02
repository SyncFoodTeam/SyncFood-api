using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.DTO.Input
{
    public class UserRegisterDTO
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }


        // Permet de caster un User en USerRegisterDTO

        public static explicit operator UserRegisterDTO(User user)
        {
            return new UserRegisterDTO { UserName = user.UserName, Email = user.Email, Password = user.Password };
        }
    }
}
