using Blog_Application.Data;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;
using Blog_Application.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog_Application.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubscribeResponse> Subscribe(Guid userId, Guid authorId)
        {
            var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == authorId && u.Role == UserRole.Author);
            if (author == null)
            {
                return SubscribeResponse.InvalidAuthor;
            }

            var existingSubscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.AuthorId == authorId);

            if (existingSubscription != null)
            {
                return SubscribeResponse.AlreadySubscribed;
            }

            var subscription = new Subscription { UserId = userId, AuthorId = authorId };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return SubscribeResponse.Success;
        }

        public async Task<SubscribeResponse> Unsubscribe(Guid userId, Guid authorId)
        {
            var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == authorId && u.Role == UserRole.Author);

            if (author == null) return SubscribeResponse.InvalidAuthor;

            var existingSubscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.AuthorId == authorId && s.UserId == userId);

            if (existingSubscription == null) return SubscribeResponse.NotYetSubscribed;

            _context.Subscriptions.Remove(existingSubscription);
            await _context.SaveChangesAsync();

            return SubscribeResponse.Success;
        }

        public async Task<List<SubscriberDto>> GetSubscribers(Guid authorId)
        {
            var author = await _context.Users.FirstOrDefaultAsync(u => u.Id == authorId && u.Role == UserRole.Author);

            if (author == null) return null;

            var subscribers = await _context.Subscriptions
                .Where(s => s.AuthorId == authorId)
                .Include(s => s.User)
                .Select(s => new SubscriberDto
                {
                    UserId = s.UserId,
                    Username = s.User.Name
                })
                .ToListAsync();

            return subscribers;
        }

        public async Task<List<SubscriptionDto>> GetSubscriptions(Guid userId)
        {
            var user = await _context.Users
                .Include(a => a.Subscriptions!)
                .ThenInclude(s => s.Author)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            var subscriptions = user.Subscriptions!
                .Select(s => new SubscriptionDto
                {
                    Id = s.Author.Id,
                    Author = s.Author.Name
                })
                .ToList();

            return subscriptions;
        }

    }
}
