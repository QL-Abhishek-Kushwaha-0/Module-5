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
        }

        public IMongoCollection<User> Users => _db.GetCollection<User>("Users");
        public IMongoCollection<Category> Categories => _db.GetCollection<Category>("Categories");
        public IMongoCollection<Post> Posts => _db.GetCollection<Post>("Posts");



    }
}
