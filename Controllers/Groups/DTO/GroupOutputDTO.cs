using SyncFoodApi.Controllers.Users.DTO;
using SyncFoodApi.Models;
using static SyncFoodApi.Controllers.Groups.GroupUtils;

namespace SyncFoodApi.Controllers.Groups.DTO
{
    public class GroupPrivateDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public float Budget { get; set; } = 0f;
        public List<UserPublicDTO> Members { get; set; } = new List<UserPublicDTO>();
        public required UserPublicDTO Owner { get; set; }
        public List<FoodContainer> FoodContainers { get; set; } = new List<FoodContainer>();
        public ShoppingList ShoppingList { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public static explicit operator GroupPrivateDTO(Group group)
        {
            return new GroupPrivateDTO
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Budget = group.Budget,
                Members = getPublicMembers(group.Members),
                Owner = (UserPublicDTO)group.Owner,
                FoodContainers = group.FoodContainers,
                ShoppingList = group.ShoppingList,
                CreationDate = group.CreationDate
            };
        }
    }

    // Utilisé pour renvoyer une liste de groupe
    public class GroupPrivateLitedDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public float Budget { get; set; } = 0f;
        public required UserPublicDTO Owner { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public static explicit operator GroupPrivateLitedDTO(Group group)
        {
            return new GroupPrivateLitedDTO
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Budget = group.Budget,
                Owner = (UserPublicDTO)group.Owner,
                CreationDate = group.CreationDate
            };
        }
    }
}
