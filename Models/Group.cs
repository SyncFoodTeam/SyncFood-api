namespace SyncFoodApi.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Budget { get; set; }
        public List<User> Members { get; set; }
        public User Owner { get; set; }
        public List<FoodContainer> foodContainers { get; set; }
        public ShoppingList ShoppingList { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
