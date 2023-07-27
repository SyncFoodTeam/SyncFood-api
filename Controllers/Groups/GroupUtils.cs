using SyncFoodApi.Controllers.Groups.DTO;
using SyncFoodApi.Controllers.Users.DTO;
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

        // Prend une liste de group et renvoi l'équivalent en GroupPrivateLiteDTO
        public static List<GroupPrivateLitedDTO> GetGroupsPrivateliteDTO(List<Group> groups)
        {
            List<GroupPrivateLitedDTO> groupsPrivateliteDTO = new List<GroupPrivateLitedDTO>();
            foreach (Group group in groups)
            {
                groupsPrivateliteDTO.Add((GroupPrivateLitedDTO)group);
            }

            return groupsPrivateliteDTO;
        }
    }
}
