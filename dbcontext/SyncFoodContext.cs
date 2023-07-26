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

            modelBuilder.Entity<Group>().HasMany(group => group.Members).WithMany(user => user.Groups);
            modelBuilder.Entity<Group>().HasOne(group => group.Owner).WithMany(user => user.ownedGroup);
            modelBuilder.Entity<FoodContainer>().HasOne(foodcontainer => foodcontainer.group).WithMany(group => group.FoodContainers);
            modelBuilder.Entity<Product>().HasMany(product => product.FoodContainers).WithMany(foodcontainer => foodcontainer.Products);
            modelBuilder.Entity<ShoppingList>().HasOne(shoppingList => shoppingList.FoodContainer).WithOne(foodContainer => foodContainer.ShoppingList);
        }
        #endregion
    }
}
