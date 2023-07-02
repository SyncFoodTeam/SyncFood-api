using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.DTO.Output
{
    public class UserPublicDTO
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Discriminator { get; set; }
        public Role Role { get; set; } = Role.USER;
        public DateTime CreationDate { get; set; }

        // Permet de caster un User en USerPublicteDTO

        public static explicit operator UserPublicDTO(User user)
        {
            return new UserPublicDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Discriminator = user.Discriminator,
                Role = user.Role,
                CreationDate = user.CreationDate
            };
        }
    }
}
