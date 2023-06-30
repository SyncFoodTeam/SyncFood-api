namespace SyncFoodApi.Models
{
    public class Group
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public float Budget { get; set; }
        public List<User> Members { get; set; } = new List<User>();
        public required User Owner { get; set; }
        public List<FoodContainer> foodContainers { get; set; } = new List<FoodContainer>();
        public ShoppingList? ShoppingList { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
