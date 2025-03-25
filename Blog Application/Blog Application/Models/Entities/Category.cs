namespace Blog_Application.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Guid AuthorId { get; set; }
        public User Author { get; set; }

        public ICollection<Post>? Posts { get; set; }
    }
}
