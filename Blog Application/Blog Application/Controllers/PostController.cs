using System.Security.Claims;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.Helper;
using Blog_Application.Utils;
using Blog_Application.Models.Entities;
using Blog_Application.Resources;
using Blog_Application.Services;
using Microsoft.AspNetCore.Authorization;
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

        // Api to Create a New Post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPost("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Create(string categoryId, PostDto postDto)
        {
            var authorIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authorIdRes == null) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var category = await _postService.CreatePost(categoryId, postDto, authorIdRes);

            if (category == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.POST_CREATE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_CREATED, category));
        }

        // API to Get a specific post

        [HttpGet("{postId}")]
        public async Task<ActionResult<ApiResponse>> GetPost(string postId)
        {
            var post = await _postService.GetPostById(postId);

            if (post == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_FETCHED, post));
        }

        // Api to update the post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPut("{postId}")]
        public async Task<ActionResult<ApiResponse>> Update(PostDto postDto, string postId)
        {
            var authorIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authorIdRes == null) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var updatedPost = await _postService.UpdatePost(postDto, postId, authorIdRes);

            if (updatedPost == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_UPDATED, updatedPost));
        }


        // API to delete a Post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpDelete("{postId}")]
        public async Task<ActionResult<ApiResponse>> Delete(string postId)
        {
            var authorIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (authorIdRes == null)
            {
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            }

            var result = await _postService.DeletePost(postId, authorIdRes);

            if (result.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (result.Equals("InvalidAuthor")) return Conflict(new ApiResponse(false, 409, ResponseMessages.POST_DELETE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_DELETE));
        }

        // API to upload the Image to Post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPatch("upload/image/{postId}")]
        public async Task<ActionResult<ApiResponse>> UploadImage(string postId, IFormFile image)
        {
            if (image == null || image.Length == 0) return BadRequest(new ApiResponse(false, 400, ResponseMessages.NO_IMAGE));

            var imageUrlRes = await _postService.UploadImage(postId, image, Request);

            if (imageUrlRes.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (imageUrlRes.Equals("InvalidImage")) return Conflict(new ApiResponse(false, 409, ResponseMessages.INVALID_IMAGE));

            return Ok(new ApiResponse(true, 200, ResponseMessages.IMAGE_UPLOADED, new { imageUrlRes }));
        }

        // Api to publish a post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPatch("{postId}/publish")]
        public async Task<ActionResult<ApiResponse>> Publish(string postId)
        {
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authorId == null) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var res = await _postService.PublishPost(postId, authorId);

            if (res.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (res.Equals("UnAuthorized")) return Conflict(new ApiResponse(false, 409, ResponseMessages.PUBLISH_CONFLICT));
            if (res.Equals("AlreadyPublished")) return Conflict(new ApiResponse(false, 409, ResponseMessages.PUBLISHED_POST_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_PUBLISHED));
        }

        // API to unpublish a post

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPatch("{postId}/unpublish")]
        public async Task<ActionResult<ApiResponse>> Unpublish(string postId)
        {
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authorId == null) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var res = await _postService.UnpublishPost(postId, authorId);

            if (res.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (res.Equals("UnAuthorized")) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNPUBLISH_CONFLICT));
            if (res.Equals("NotPublishedYet")) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNPUBLISHED_POST_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_UNPUBLISHED));
        }

        // Api to get all Posts

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            var posts = await _postService.GetAllPosts();

            if (!posts.Any()) return Ok(new ApiResponse(true, 200, ResponseMessages.NO_POSTS));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POSTS_FETCHED, posts));
        }


        // API to get the posts of a specific author

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpGet("users/{authorId}")]
        public async Task<ActionResult<ApiResponse>> GetAuthorPosts(string authorId)
        {
            var posts = await _postService.GetAuthorPosts(authorId);    // Will always return a list 

            if (posts == null) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            if (!posts.Any()) return Ok(new ApiResponse(true, 404, ResponseMessages.NO_PUBLISHED_POSTS));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POSTS_FETCHED, posts));
        }


        // Api to Fetch all the Posts Under a Specific Category

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse>> GetPosts(string categoryId)
        {
            var posts = await _postService.GetCategoryPosts(categoryId);

            if (posts == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORY));
            if (!posts.Any()) return Ok(new ApiResponse(true, 200, ResponseMessages.NO_POSTS));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POSTS_FETCHED, posts));
        }
    }
}
