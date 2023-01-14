namespace SyncFoodApi.Models
{
    public class ShoppingList
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public List<Product> Products { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
