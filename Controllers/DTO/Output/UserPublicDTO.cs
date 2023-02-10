using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.DTO.Output
{
    public class UserPublicDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Discriminator { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }

        // Permet de caster un User en USerPrivateDTO

        public static explicit operator UserPublicDTO(User user)
        {
            return new UserPublicDTO
            {
                UserName = user.UserName,
                Discriminator = user.Discriminator,
                CreationDate = user.CreationDate
            };
        }
    }
}
