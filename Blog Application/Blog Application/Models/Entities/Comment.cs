using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Blog_Application.Models.Entities
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Content { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
    }
}
