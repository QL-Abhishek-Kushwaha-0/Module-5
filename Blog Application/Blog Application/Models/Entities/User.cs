namespace Blog_Application.Models.Entities
{
    public enum UserRole
    {
        Viewer,
        Author,
    }
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public required string Email { get; set; }
        public string Password { get; set; }
        public string? Username { get; set; }
        public UserRole Role { get; set; } 


        public ICollection<Category>? Categories { get; set; }
        public ICollection<Post>? Posts { get; set; }
    }
}
