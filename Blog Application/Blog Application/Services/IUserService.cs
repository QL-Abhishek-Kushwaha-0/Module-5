using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;

namespace Blog_Application.Services
{
    public interface IUserService
    {
        Task<SubscribeResponse> Subscribe(string userId, string authorId);
        Task<SubscribeResponse> Unsubscribe(string userId, string authorId);
        Task<List<SubscriberDto>> GetSubscribers(string authorId);
        Task<List<SubscriptionDto>> GetSubscriptions(string userId);
    }
}
