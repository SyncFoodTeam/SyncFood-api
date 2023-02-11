namespace SyncFoodApi.Models
{
    public class FoodContainer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new List<Product>();
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
