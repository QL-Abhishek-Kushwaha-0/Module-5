using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Models.Entities;

namespace Blog_Application.Services
{
    public interface IPostService
    {
        Task<PostResponseDto> CreatePost(string categoryId, PostDto postDto, string authorId);
        Task<PostResponseDto> GetPostById(string postId);
        Task<PostResponseDto> UpdatePost(PostDto postDto, string postId, string authorId);
        Task<string> DeletePost(string postId, string authorId);
        Task<string> UploadImage(string postId, IFormFile image, HttpRequest request);
        //Task<List<PostResponseDto>> GetAllPosts();
        //Task<List<PostResponseDto>> GetAuthorPosts(Guid authorId);
        //Task<List<PostResponseDto>> GetCategoryPosts(int categoryId);

        //Task<string> PublishPost(int postId, Guid authorId);
        //Task<string> UnpublishPost(int postId, Guid authorId);
    }
}
