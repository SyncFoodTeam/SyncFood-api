﻿namespace SyncFoodApi.Models
{
    public class FoodContainer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
