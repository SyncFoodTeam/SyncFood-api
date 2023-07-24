using SyncFoodApi.Controllers.DTO.Output;
using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Groups
{
    public static class GroupUtils
    {
        // Prend une liste de user et renvoi l'équivalent en UserPublicDTO
        public static List<UserPublicDTO> getPublicMembers(List<User> members)
        {
            List<UserPublicDTO> userPublicDTOs = new List<UserPublicDTO>();
            foreach (User user in members)
            {
                userPublicDTOs.Add((UserPublicDTO)user);
            }

            return userPublicDTOs;
        }
    }
}
