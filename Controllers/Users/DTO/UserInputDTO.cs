using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Users.DTO
{
    public class UserRegisterDTO
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }


        // Permet de caster un User en USerRegisterDTO

       /* public static explicit operator UserRegisterDTO(User user)
        {
            return new UserRegisterDTO { UserName = user.UserName, Email = user.Email, Password = user.Password };
        }*/
    }

    public class UserLoginDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }


        // Permet de caster un User en USerLoginDTO

        /*public static explicit operator UserLoginDTO(User user)
        {
            return new UserLoginDTO { Email = user.Email, Password = user.Password };
        }*/
    }

    public class UserUpdateDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        // Permet de caster un User en USerUpdateDTO

        /*public static explicit operator UserUpdateDTO(User user)
        {
            return new UserUpdateDTO { UserName = user.UserName, Email = user.Email, Password = user.Password };
        }*/
    }


}
