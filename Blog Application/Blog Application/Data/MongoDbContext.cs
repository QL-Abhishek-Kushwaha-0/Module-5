using Blog_Application.Models.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Blog_Application.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _db = client.GetDatabase(settings.Value.DatabaseName);

            CreateIndexes();
        }

        public IMongoCollection<User> Users => _db.GetCollection<User>("Users");
        public IMongoCollection<Category> Categories => _db.GetCollection<Category>("Categories");
        public IMongoCollection<Post> Posts => _db.GetCollection<Post>("Posts");
        public IMongoCollection<Like> Likes => _db.GetCollection<Like>("Likes");
        public IMongoCollection<Comment> Comments => _db.GetCollection<Comment>("Comments");
        public IMongoCollection<Subscription> Subscriptions => _db.GetCollection<Subscription>("Subscriptions");

        private void CreateIndexes()
        {
            // Creating Index at Email Field to make lookups faster in Authentication
            var emailIndex = Builders<User>.IndexKeys.Ascending(u => u.Email);
            Users.Indexes.CreateOne(new CreateIndexModel<User>(emailIndex, new CreateIndexOptions { Unique = true }));

            // Creating Composite Index at PostId and UserId in Likes Collection to ensure Uniqueness and faster lookups
            var likeIndex = Builders<Like>.IndexKeys.Ascending(l => l.PostId).Descending(l => l.UserId);
            Likes.Indexes.CreateOne(new CreateIndexModel<Like>(likeIndex, new CreateIndexOptions { Unique = true }));

            // Creating Composite Index at AuthorId and UserId in Subscription Collection to ensure Uniqueness and faster lookups
            var subscriptionIndex = Builders<Subscription>.IndexKeys.Ascending(s => s.UserId).Ascending(s => s.AuthorId);
            Subscriptions.Indexes.CreateOne(new CreateIndexModel<Subscription>(subscriptionIndex, new CreateIndexOptions { Unique = true }));
        }

    }

}