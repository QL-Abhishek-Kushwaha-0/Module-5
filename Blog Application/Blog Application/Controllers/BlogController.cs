using System.Security.Claims;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.Enums;
using Blog_Application.Helper;
using Blog_Application.Utils;
using Blog_Application.Resources;
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

        // API to Like a Blog Post
        /*
            <summary>
                Like a blog post
            </summary>
            <param name="postId">The ID of the post of type 'int' to like</param>
            <returns>Returns a success message if the post is liked successfully</returns>
         */
        [HttpPost("like/{postId}")]
        public async Task<ActionResult<ApiResponse>> Like(string postId)
        {
            var userIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdRes == null) return Unauthorized(new ApiResponse(false, 401, ResponseMessages.LOGIN_TO_INTERACT));

            var result = await _blogService.LikePost(postId, userIdRes);

            if (result == LikeResponse.NotFound) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (result == LikeResponse.AlreadyLiked) return Conflict(new ApiResponse(false, 409, ResponseMessages.LIKE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.LIKE_SUCCESS));
        }

        // API to Unlike a Blog Post
        /*
            <summary>
                Unlike a blog post
            </summary>
            <param name="postId">The ID of the post of type 'int' to unlike</param>
            <returns>Returns a success message if the post is unliked successfully</returns>
         */
        [HttpDelete("unlike/{postId}")]
        public async Task<ActionResult<ApiResponse>> Unlike(string postId)
        {
            var userIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdRes == null) return Unauthorized(new ApiResponse(false, 401, ResponseMessages.LOGIN_TO_INTERACT));

            var result = await _blogService.UnlikePost(postId, userIdRes);

            if (result == LikeResponse.NotFound) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (result == LikeResponse.NotYetLiked) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNLIKE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.UNLIKE_SUCCESS));
        }


        // API to Comment on a Blog Post
        /*
            <summary>
                Comment on a blog post
            </summary>
            <param name="commentDto">The comment details</param>
            <param name="postId">The ID of the post of type 'int' to comment on</param>
            <returns>Returns a success message if the comment is added successfully</returns>
         */
        [HttpPost("comment/{postId}")]
        public async Task<ActionResult<ApiResponse>> Comment(CommentDto commentDto, string postId)
        {
            var userIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdRes == null) return Unauthorized(new ApiResponse(false, 401, ResponseMessages.LOGIN_TO_INTERACT));

            var result = await _blogService.Comment(postId, userIdRes, commentDto);

            if (result == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));

            return Ok(new ApiResponse(true, 200, ResponseMessages.COMMENT_SUCCESS, result));
        }


        // API to Delete a Comment on a Blog Post
        /*
            <summary>
                Delete a comment on a blog post
            </summary>
            <param name="postId">The ID of the post to delete the comment from</param>
            <param name="commentId">The ID of the comment to delete</param>
            <returns>Returns a success message if the comment is deleted successfully</returns>
            <remarks>
                    <para>Note: Only the author of the comment can delete it.</para>
            </remarks>
         */
        [HttpDelete("{postId}/comment/{commentId}")]
        public async Task<ActionResult<ApiResponse>> Delete(string postId, string commentId)
        {
            var userIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdRes == null) return Unauthorized(new ApiResponse(false, 401, ResponseMessages.LOGIN_TO_INTERACT));

            var result = await _blogService.DeleteComment(postId, commentId, userIdRes);

            if (result == CommentResponse.NotFound) return NotFound(new ApiResponse(false, 404, ResponseMessages.COMMENT_NOT_FOUND));
            if (result == CommentResponse.Unauthorized) return Conflict(new ApiResponse(false, 409, ResponseMessages.COMMENT_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.COMMENT_DELETE_SUCCESS));
        }

        // API to Get all Comments on a Blog Post
        /*
            <summary>
                Get all comments on a blog post
            </summary>
            <param name="postId">The ID of the post to get comments for</param>
            <returns>Returns a list of comments if found</returns>
         */
        [HttpGet("comment/{postId}")]
        public async Task<ActionResult<ApiResponse>> GetComments(string postId)
        {
            var comments = await _blogService.GetPostComments(postId);

            if (comments == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (!comments.Any()) return Ok(new ApiResponse(true, 200, ResponseMessages.NO_COMMENTS));

            return Ok(new ApiResponse(true, 200, ResponseMessages.COMMENTS_FETCHED, comments));
        }

    }
}
