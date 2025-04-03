namespace Blog_Application.Models.Entities
{
    public class Subscription
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid AuthorId { get; set; }
        public User Author { get; set; }
    }
}
