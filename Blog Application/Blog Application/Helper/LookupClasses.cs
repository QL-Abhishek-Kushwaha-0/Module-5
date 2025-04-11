using Blog_Application.Models.Entities;

namespace Blog_Application.Helper
{
    public class LookupClasses
    {
        public class CategoryWithAuthor:Category
        {
            public List<User>? Author { get; set; }
        }

        public class PostWithCategory : Post
        {
            public List<Category>? Category { get; set; }
        }
        public class PostWithAuthor : PostWithCategory
        {
            public List<User>? Author { get; set; }
        }

        public class PostWithPostOwner : Post
        {
            public List<User>? Owner { get; set; }
        }

        public class CommentWithUser : Comment
        {
            public List<User>? Author { get; set; }
        }
        public class SubscribersAndSubscriptionLookup
        {
            public string Id { get; set; }
            public string UserId { get; set; }
            public string AuthorId { get; set; }
            public List<User>? Subscriber { get; set; }
            public List<User>? Subscriptions { get; set; }
        }
    }
}
