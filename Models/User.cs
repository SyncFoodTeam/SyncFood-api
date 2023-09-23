namespace SyncFoodApi.Models
{
    public enum Role
    {
        USER,ADMIN
    }
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Discriminator { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public Role Role { get; set; } = Role.USER;
        public string Token { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        // DB
        public List<Group> OwnedGroup { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();
    }
}
