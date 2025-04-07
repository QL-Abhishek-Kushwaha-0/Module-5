using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Models.Entities;

namespace Blog_Application.Services
{
    public interface IPostService
    {
        Task<List<PostResponseDto>> GetAllPosts();
        Task<List<PostResponseDto>> GetCategoryPosts(int categoryId);
        Task<PostResponseDto> GetPostById(int postId);
        Task<PostResponseDto> CreatePost(int categoryId, PostDto postDto, Guid authorId);

        Task<PostResponseDto> UpdatePost(PostDto postDto, int postId, Guid authorId);

        Task<string> UploadImage(int postId, IFormFile image, HttpRequest request);

        Task<string> PublishPost(int postId, Guid authorId);
        Task<string> UnpublishPost(int postId, Guid authorId);
        Task<string> DeletePost(int postId, Guid authorId);
    }
}
