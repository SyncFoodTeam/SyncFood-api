using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SyncFoodApi.Models;

namespace SyncFoodApi.dbcontext
{
    public class SyncFoodContext : DbContext
    {



        public string DbPath { get; }
        public SyncFoodContext() 
        {
            DbPath = "SyncFood.db";
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<FoodContainer> FoodContainers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ShoppingList> ShoppingLists { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
       
        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relations explicite entre les différents modèles

            // Group <=> User
            modelBuilder.Entity<Group>().HasMany(group => group.Members).WithMany(user => user.Groups);
            modelBuilder.Entity<Group>().HasOne(group => group.Owner).WithMany(user => user.OwnedGroup);

            // Group <=> ShoppingList
            modelBuilder.Entity<Group>().HasOne(group => group.ShoppingList).WithOne(shoppingList => shoppingList.Group).HasForeignKey<ShoppingList>(shoppingList => shoppingList.GroupId);

            // ShoppingList <=> Product
            modelBuilder.Entity<ShoppingList>().HasMany(shoppingList => shoppingList.Products).WithOne(product => product.ShoppingList);

            // FoodContainer <=> Group
            modelBuilder.Entity<FoodContainer>().HasOne(foodcontainer => foodcontainer.group).WithMany(group => group.FoodContainers);

            // FoodContainer <=> Product
            modelBuilder.Entity<FoodContainer>().HasMany(foodContainer => foodContainer.Products).WithOne(product => product.FoodContainer);
            
        }
        #endregion
    }
}
