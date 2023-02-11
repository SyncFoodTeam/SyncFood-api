namespace SyncFoodApi.Models
{
    public enum NutriScore
    {
        A,B,C,D,E
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public int BarCode { get; set; }
        public NutriScore NutriScore { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
