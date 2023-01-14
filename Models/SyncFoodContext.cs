using Microsoft.EntityFrameworkCore;

namespace SyncFoodApi.Models
{
    public class SyncFoodContext : DbContext
    {
        public SyncFoodContext(DbContextOptions<SyncFoodContext> options) : base(options) 
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<FoodContainer> FoodContainers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ShoppingList> ShoppingLists { get; set; } = null!;
       
    }
}
