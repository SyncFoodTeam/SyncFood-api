using SyncFoodApi.dbcontext;

namespace SyncFoodApi.Models
{
    public class FoodContainer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // DB
        public Group group { get; set; }

        // fonction pour vider le foodcontainer (utile pour la suppression en cascade)
        public void Empty(SyncFoodContext _context)
        {
            foreach (Product product in this.Products)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            this.Products = new List<Product>();

            _context.SaveChanges();
        }
    }
}
