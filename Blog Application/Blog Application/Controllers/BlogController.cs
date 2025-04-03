using System.Security.Claims;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.Enums;
using Blog_Application.Helper;
using Blog_Application.Middlewares;
using Blog_Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Application.Controllers
{
    [Route("api/blogs")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost("like/{postId}")]
        public async Task<ActionResult<ApiResponse>> Like(int postId)
        {
            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (userIdRes == Guid.Empty) return Unauthorized(new ApiResponse(false, 401, "Please Login First to Like the Post!!!"));

            var result = await _blogService.LikePost(postId, userIdRes);

            if (result == LikeResponse.NotFound) return NotFound(new ApiResponse(false, 404, "Post Not Found!!!"));
            if (result == LikeResponse.AlreadyLiked) return Conflict(new ApiResponse(false, 409, "Post has already been Liked!!!"));

            return Ok(new ApiResponse(true, 200, "Post Liked Successfully..."));
        }

        [HttpDelete("unlike/{postId}")]
        public async Task<ActionResult<ApiResponse>> Unlike(int postId)
        {
            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (userIdRes == Guid.Empty) return Unauthorized(new ApiResponse(false, 401, "Please Login First to Like the Post!!!"));

            var result = await _blogService.UnlikePost(postId, userIdRes);

            if (result == LikeResponse.NotFound) return NotFound(new ApiResponse(false, 404, "Post Not Found!!!"));
            if (result == LikeResponse.NotYetLiked) return Conflict(new ApiResponse(false, 409, "Post has not been Liked Yet!!!"));

            return Ok(new ApiResponse(true, 200, "Post unliked Successfully..."));
        }

        [HttpPost("comment/{postId}")]
        public async Task<ActionResult<ApiResponse>> Comment([FromBody] CommentDto commentDto, int postId)
        {
            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (userIdRes == Guid.Empty) return Unauthorized(new ApiResponse(false, 401, "Login first to Comment!!!!"));

            var result = await _blogService.Comment(postId, userIdRes, commentDto);

            if (result == null) return NotFound(new ApiResponse(false, 404, "No Post Found!!!!"));

            return Ok(new ApiResponse(true, 200, "Commented Successfully...", result));
        }

        [HttpDelete("{postId}/comment/{commentId}")]
        public async Task<ActionResult<ApiResponse>> Delete(int postId, int commentId)
        {
            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (userIdRes == Guid.Empty) return Unauthorized(new ApiResponse(false, 401, "Login to Continue!!!"));

            var result = await _blogService.DeleteComment(postId, commentId, userIdRes);

            if (result == CommentResponse.NotFound) return NotFound(new ApiResponse(false, 404, "No such Comment Found!!!"));
            if (result == CommentResponse.Unauthorized) return Conflict(new ApiResponse(false, 409, "Cannot Delete Comment posted by others!!!!"));

            return Ok(new ApiResponse(true, 200, "Comment Deleted Successfully..."));
        }

        [HttpGet("comment/{postId}")]
        public async Task<ActionResult<ApiResponse>> GetComments(int postId)
        {
            var comments = await _blogService.GetPostComments(postId);

            if(comments.Any() == false) return NotFound(new ApiResponse(false, 404, "No Comments Found for this Post!!!"));

            return Ok(new ApiResponse(true, 200, "Comments for this post fetched Successfully!!!", comments));
        }

    }
}
