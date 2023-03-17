namespace SyncFoodApi.Models
{
    public enum Role
    {
        USER,ADMIN
    }
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Discriminator { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.USER;
        public string Token { get; set; } = string.Empty; 
        public DateTime CreationDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
