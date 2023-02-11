namespace SyncFoodApi.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new List<Product>();
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
