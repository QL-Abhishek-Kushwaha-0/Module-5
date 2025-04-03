using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;
using Blog_Application.Models.Entities;

namespace Blog_Application.Services
{
    public interface IUserService
    {
        Task<SubscribeDto> Subscribe(Guid userId, Guid authorId);
        Task<SubscribeDto> Unsubscribe(Guid userId, Guid authorId);
        Task<List<SubscriberDto>> GetSubscribers(Guid authorId);
        Task<List<SubscriptionDto>> GetSubscriptions(Guid userId);
    }
}
