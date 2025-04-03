using System.Security.Claims;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.Helper;
using Blog_Application.Middlewares;
using Blog_Application.Models.Entities;
using Blog_Application.Resources;
using Blog_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Application.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }


        // Api to get all Posts
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            var posts = await _postService.GetAllPosts();

            return Ok(new ApiResponse(true, 200, ResponseMessages.POSTS_FETCHED, posts));
        }


        // Api to Fetch all the Posts Under a Specific Category

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse>> GetPosts(int categoryId)
        {
            var posts = await _postService.GetCategoryPosts(categoryId);

            return Ok(new ApiResponse(true, 200, ResponseMessages.POSTS_FETCHED, posts));
        }


        [HttpGet("{postId}")]
        public async Task<ActionResult<ApiResponse>> GetPost(int postId)
        {
            var post = await _postService.GetPostById(postId);

            if (post == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_FETCHED, post));
        }


        // Api to update the post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPut("{postId}")]
        public async Task<ActionResult<ApiResponse>> Update([FromBody] PostDto postDto, int postId)
        {
            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (authorIdRes == Guid.Empty) return NotFound(new ApiResponse(false, 404, ResponseMessages.INVALID_GUID));

            Guid authorId = authorIdRes;

            var updatedPost = await _postService.UpdatePost(postDto, postId, authorId);

            if (updatedPost == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_UPDATED, updatedPost));
        }

        // Api to Create a New Post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPost("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Create([FromRoute] int categoryId, [FromBody] PostDto postDto)
        {
            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (authorIdRes == Guid.Empty) return NotFound(new ApiResponse(false, 404, ResponseMessages.INVALID_GUID));

            Guid authorId = authorIdRes;

            var category = await _postService.CreatePost(categoryId, postDto, authorId);

            if (category == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.POST_CREATE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_CREATED, category));
        }

        // Api to publish a post
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPatch("{postId}/publish")]
        public async Task<ActionResult<ApiResponse>> Publish(int postId)
        {
            Guid authorId = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
   
            var res = await _postService.PublishPost(postId, authorId);

            if (res.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (res.Equals("UnAuthorized")) return Conflict(new ApiResponse(false, 409, ResponseMessages.PUBLISH_CONFLICT));
            if (res.Equals("AlreadyPublished")) return Conflict(new ApiResponse(false, 409, ResponseMessages.PUBLISHED_POST_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_PUBLISHED));
        }


        // API to unpublish a post
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPatch("{postId}/unpublish")]
        public async Task<ActionResult<ApiResponse>> Unpublish(int postId)
        {
            Guid authorId = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            var res = await _postService.UnpublishPost(postId, authorId);

            if (res.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (res.Equals("UnAuthorized")) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNPUBLISH_CONFLICT));
            if (res.Equals("NotPublishedYet")) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNPUBLISHED_POST_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_UNPUBLISHED));
        }


        // API to delete a Post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpDelete("{postId}")]
        public async Task<ActionResult<ApiResponse>> Delete(int postId)
        {
            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (authorIdRes == Guid.Empty)
            {
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            }
            Guid authorId = authorIdRes;

            var result = await _postService.DeletePost(postId, authorId);

            if (result.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (result.Equals("InvalidAuthor")) return Conflict(new ApiResponse(false, 409, ResponseMessages.POST_DELETE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_DELETE));
        }
    }
}
