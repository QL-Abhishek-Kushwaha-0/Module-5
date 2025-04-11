using Blog_Application.Data;
using Blog_Application.DTO;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;
using Blog_Application.Helper;
using Blog_Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Blog_Application.Services
{
    public class UserService : IUserService
    {
        private readonly MongoDbContext _context;
        public UserService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<SubscribeResponse> Subscribe(string userId, string authorId)
        {
            var author = await _context.Users.Find(u => u.Id == authorId && u.Role == UserRole.Author).FirstOrDefaultAsync();
            if (author == null)
            {
                return SubscribeResponse.InvalidAuthor;
            }

            var existingSubscription = await _context.Subscriptions
                .Find(s => s.UserId == userId && s.AuthorId == authorId).FirstOrDefaultAsync();

            if (existingSubscription != null)
            {
                return SubscribeResponse.AlreadySubscribed;
            }

            var subscription = new Subscription { UserId = userId, AuthorId = authorId };
            await _context.Subscriptions.InsertOneAsync(subscription);

            return SubscribeResponse.Success;
        }

        public async Task<SubscribeResponse> Unsubscribe(string userId, string authorId)
        {
            var author = await _context.Users.Find(u => u.Id == authorId && u.Role == UserRole.Author).FirstOrDefaultAsync();

            if (author == null) return SubscribeResponse.InvalidAuthor;

            var existingSubscription = await _context.Subscriptions.Find(s => s.AuthorId == authorId && s.UserId == userId).FirstOrDefaultAsync();

            if (existingSubscription == null) return SubscribeResponse.NotYetSubscribed;

            await _context.Subscriptions.DeleteOneAsync(Builders<Subscription>.Filter.Eq(s => s.Id, existingSubscription.Id));

            return SubscribeResponse.Success;
        }

        public async Task<List<SubscriberDto>> GetSubscribers(string authorId)
        {
            var author = await _context.Users.Find(u => u.Id == authorId && u.Role == UserRole.Author).FirstOrDefaultAsync();

            if (author == null) return null;

            var subscribers = await _context.Subscriptions
                .Aggregate()
                .Match(s => s.AuthorId == authorId)
                .Lookup<Subscription, User, LookupClasses.SubscribersAndSubscriptionLookup>(
                    _context.Users,
                    s => s.UserId,
                    u => u.Id,
                    res => res.Subscriber
                )
                .Project(s => new SubscriberDto
                {
                    UserId = s.Subscriber!.FirstOrDefault()!.Id,
                    Username = s.Subscriber!.FirstOrDefault()!.Name
                })
                .ToListAsync();

            return subscribers;
        }

        public async Task<List<SubscriptionDto>> GetSubscriptions(string userId)
        {
            var user = await _context.Users
                .Find(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            var subscriptions = await _context.Subscriptions
                .Aggregate()
                .Match(s => s.UserId == userId)
                .Lookup<Subscription, User, LookupClasses.SubscribersAndSubscriptionLookup>(
                    _context.Users,
                    s => s.AuthorId,
                    u => u.Id,
                    res => res.Subscriptions
                )
                .Project(s => new SubscriptionDto
                {
                    Id = s.Subscriptions!.FirstOrDefault()!.Id,
                    Author = s.Subscriptions!.FirstOrDefault()!.Name
                })
                .ToListAsync();

            return subscriptions;
        }

    }
}
