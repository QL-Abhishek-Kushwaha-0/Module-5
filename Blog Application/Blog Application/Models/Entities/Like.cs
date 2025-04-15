using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Blog_Application.Models.Entities
{
    public class Like   
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        public string PostId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
    }
}