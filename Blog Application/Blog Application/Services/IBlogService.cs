using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Enums;

namespace Blog_Application.Services
{
    public interface IBlogService
    {
        Task<LikeResponse> LikePost(int postId, Guid userId);
        Task<LikeResponse> UnlikePost(int postId, Guid userId);
        Task<CommentResponseDto> Comment(int postId, Guid userId, CommentDto commentDto);
        Task<CommentResponse> DeleteComment(int postId, int commentId, Guid authorId);
        Task<List<CommentResponseDto>> GetPostComments(int postId);
    }
}
