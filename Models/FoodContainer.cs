namespace SyncFoodApi.Models
{
    public class FoodContainer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
