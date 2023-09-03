using SyncFoodApi.dbcontext;

namespace SyncFoodApi.Models
{
    public class Group
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public float Budget { get; set; } = 0f;
        public List<User> Members { get; set; } = new List<User>();
        public User Owner { get; set; }
        public List<FoodContainer> FoodContainers { get; set; } = new List<FoodContainer>();
        public ShoppingList ShoppingList { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // fonction pour vider le group (utile pour la suppression en cascade)
        public void Empty(SyncFoodContext _context)
        {
            foreach (FoodContainer foodcontainer in this.FoodContainers)
            {
                foodcontainer.Empty(_context);
                _context.Remove(foodcontainer);
            }

            this.Owner = null;
            this.ShoppingList = null;
            this.Members = null;

            _context.SaveChanges();
        }
    }
}
