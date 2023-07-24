using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Users.DTO.Input
{
    public class UserUpdateDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }


        // Permet de caster un User en USerLoginDTO

        public static explicit operator UserUpdateDTO(User user)
        {
            return new UserUpdateDTO { UserName = user.UserName, Email = user.Email, Password = user.Password };
        }
    }
}
