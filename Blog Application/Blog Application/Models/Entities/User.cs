using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Blog_Application.Models.Entities
{
    public enum UserRole
    {
        Viewer,
        Author,
    }
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Username { get; set; }
        public UserRole Role { get; set; } 
    }
}
