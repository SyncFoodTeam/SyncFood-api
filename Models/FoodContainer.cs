namespace SyncFoodApi.Models
{
    public class FoodContainer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public ShoppingList ShoppingList { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // DB
        public Group group { get; set; }
    }
}
