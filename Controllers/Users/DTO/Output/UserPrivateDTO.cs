using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.DTO.Output
{
    public class UserPrivateDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Discriminator { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.USER;
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Permet de caster un User en USerPrivateDTO

        public static explicit operator UserPrivateDTO(User user)
        {
            return new UserPrivateDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Discriminator = user.Discriminator,
                Email = user.Email,
                Token = user.Token,
                Role = user.Role,
                CreationDate = user.CreationDate,
                UpdatedDate = user.UpdatedDate
            };
        }
    }
}
