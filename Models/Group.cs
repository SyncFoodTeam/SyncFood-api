namespace SyncFoodApi.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Budget { get; set; }
        public List<User> Members { get; set; } = new List<User>();
        public User Owner { get; set; } = new User();
        public List<FoodContainer> foodContainers { get; set; }
        public ShoppingList ShoppingList { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
