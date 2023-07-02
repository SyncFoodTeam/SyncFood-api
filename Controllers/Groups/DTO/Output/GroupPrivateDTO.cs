using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Groups.DTO.Output
{
    public class GroupPrivateDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public float Budget { get; set; } = 0f;
        public List<User> Members { get; set; } = new List<User>();
        public required User Owner { get; set; }
        public List<FoodContainer> foodContainers { get; set; } = new List<FoodContainer>();
        public ShoppingList? ShoppingList { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public static explicit operator GroupPrivateDTO(Group group)
        {
            return new GroupPrivateDTO
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                Budget = group.Budget,
                Members = group.Members,
                Owner = group.Owner,
                foodContainers = group.foodContainers,
                ShoppingList = group.ShoppingList,
                CreationDate = group.CreationDate
            };
        }
    }
}
