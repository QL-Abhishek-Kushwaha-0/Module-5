namespace Blog_Application.Models.Entities
{
    public class Like   
    {
        public int PostId { get; set; }
        public Post Post { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}