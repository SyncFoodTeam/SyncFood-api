namespace SyncFoodApi.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public required string Nom { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
