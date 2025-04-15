using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;

namespace Blog_Application.Services
{
    public interface IBlogService
    {
        Task<LikeResponse> LikePost(string postId, string userId);
        Task<LikeResponse> UnlikePost(string postId, string userId);
        Task<CommentResponseDto> Comment(string postId, string userId, CommentDto commentDto);
        Task<CommentResponse> DeleteComment(string postId, string commentId, string authorId);
        Task<List<CommentResponseDto>> GetPostComments(string postId);
    }
}
