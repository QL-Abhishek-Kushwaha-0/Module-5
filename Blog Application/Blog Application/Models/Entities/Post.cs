using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Blog_Application.Models.Entities
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; }


        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }


        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }
    }
}
