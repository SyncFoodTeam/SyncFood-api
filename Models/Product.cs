namespace SyncFoodApi.Models
{
    public enum NutriScore
    {
        A,B,C,D,E
    }

    /*
     * - fruit
     * - Légum
     * - Viande
     * - Poisson
     * - Surgelé
     * - Boite de conserve
     * - Gateaux
     */
    /*public enum Type_produit
    {
        Fruit,Vegetable,Meat,Fish,Frozen_food,Tinned_food,Cakes
    }*/
    public class Product
    {
        public int Id { get; set; }
        /*public required string Name { get; set; }*/
        public required int BarCode { get; set; }
        public required float Price { get; set; }
        /*public NutriScore NutriScore { get; set; }*/
        /*public float nutritionalValue { get; set; }*/
        public required int Quantity { get; set; }
        public required DateTime ExpirationDate { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // DB
        public FoodContainer FoodContainer { get; set; }
        public ShoppingList ShoppingList { get; set; }
    }
}
