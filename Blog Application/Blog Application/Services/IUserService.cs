using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;

namespace Blog_Application.Services
{
    public interface IUserService
    {
        Task<SubscribeResponse> Subscribe(Guid userId, Guid authorId);
        Task<SubscribeResponse> Unsubscribe(Guid userId, Guid authorId);
        Task<List<SubscriberDto>> GetSubscribers(Guid authorId);
        Task<List<SubscriptionDto>> GetSubscriptions(Guid userId);
    }
}
