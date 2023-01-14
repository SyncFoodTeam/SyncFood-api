namespace SyncFoodApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
