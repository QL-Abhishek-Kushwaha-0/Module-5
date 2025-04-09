
namespace Blog_Application.Models.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid AuthorId { get; set; }
        public User Author { get; set; }

        public ICollection<Like>? Likes { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
