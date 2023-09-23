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


        public void Empty(SyncFoodContext _context)
        {
            var products = _context.Products.Where(x => x.FoodContainer.Id == this.Id);

            foreach (Product product in products)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

        }
    }
}
