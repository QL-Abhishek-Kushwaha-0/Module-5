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

        // API to get all Published Posts
        /*
            <summary>
                Get all posts
            </summary>
            <returns>Returns API Response containing Success, Status Code , Message and Posts data</returns>
                <remarks>
                    <para>Note: Only Published posts will be fetched.</para>
                </remarks>
        */
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            var posts = await _postService.GetAllPosts();

            if (!posts.Any()) return Ok(new ApiResponse(true, 200, ResponseMessages.NO_PUBLISHED_POSTS));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POSTS_FETCHED, posts));
        }

        
        // API to get all posts of an Author
        /*
            <summary>
                Get all posts by a specific author
            </summary>
            <param name="authorId">The ID of the author</param>
            <returns>Returns API Response containing Success, Status Code , Message and Author's Posts data</returns>
                <remarks>
                    <para>Note: Only Authors can view their own posts and will contain unpublished Posts too.</para>
                </remarks>
        */

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpGet("users/{authorId:guid}")]
        public async Task<ActionResult<ApiResponse>> GetAuthorPosts(Guid authorId)
        {
            var posts = await _postService.GetAuthorPosts(authorId);    // Will always return a list 

            if (!posts.Any()) return Ok(new ApiResponse(true, 404, ResponseMessages.NO_PUBLISHED_POSTS));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POSTS_FETCHED, posts));
        }


        // API to get all posts
        /*
            <summary>
                Get all posts under a specific category
            </summary>
            <param name="categoryId">The ID of the category from URL</param>
            <returns>Returns API Response containing Success, Status Code , Message and Category's Posts data</returns>
            <remarks>
                <para>Note: Only Published posts will be fetched.</para>
            </remarks>
        */
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse>> GetPosts(int categoryId)
        {
            var posts = await _postService.GetCategoryPosts(categoryId);

            if (posts == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORY));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POSTS_FETCHED, posts));
        }

        // API to get a Post by its ID
        /* 
            <summary>
                Get a specific post by its ID
            </summary>
            <param name="postId">The ID of the post</param>
            <returns>Returns API Response containing Success, Status Code , Message and Post data</returns>
        */
        [HttpGet("{postId}")]
        public async Task<ActionResult<ApiResponse>> GetPost(int postId)
        {
            var post = await _postService.GetPostById(postId);

            if (post == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_FETCHED, post));
        }


        // Api to update the post
        /* 
            <summary>
                Update a specific post by its ID
            </summary>
            <param name="postId">The ID of the post</param>
            <param name="postDto">The updated post details</param>
            <returns>Returns API Response containing Success, Status Code, Message and Updated Post data</returns>
                <remarks>
                    <para>Note: Only the author of the post can update it.</para>
                </remarks>
         */
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPut("{postId}")]
        public async Task<ActionResult<ApiResponse>> Update(PostDto postDto, int postId)
        {
            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (authorIdRes == Guid.Empty) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var updatedPost = await _postService.UpdatePost(postDto, postId, authorIdRes);

            if (updatedPost == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_UPDATED, updatedPost));
        }

        // API to upload an Image
        /*
            <summary>
                Upload an image to a specific post by its ID
            </summary>
            <param name="postId">The ID of the post</param>
            <param name="image">The image file to be uploaded</param>
            <returns>Returns API Response containing Success, Status Code , Message and Image URL</returns>
                <remarks>
                    <para>Note: Only the author of the post can upload an image.</para>
                </remarks>
         */
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPatch("upload/image/{postId}")]
        public async Task<ActionResult<ApiResponse>> UploadImage(int postId, IFormFile image)
        {
            if (image == null || image.Length == 0) return BadRequest(new ApiResponse(false, 400, ResponseMessages.NO_IMAGE));

            var imageUrlRes = await _postService.UploadImage(postId, image, Request);

            if (imageUrlRes.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (imageUrlRes.Equals("InvalidImage")) return Conflict(new ApiResponse(false, 409, ResponseMessages.INVALID_IMAGE));

            return Ok(new ApiResponse(true, 200, ResponseMessages.IMAGE_UPLOADED, new { imageUrlRes }));
        }

        // API to Create a new Post
        /* 
            <summary>
                Create a new post under a specific category
            </summary>
            <param name="categoryId">The ID of the category</param>
            <param name="postDto">The details of the post to be created</param>
            <returns>Returns API Response containing Success, Status Code , Message and Created Post data</returns>
                <remarks>
                    <para>Note: Only Authors can create it.</para>
                </remarks>
         */
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPost("category/{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Create(int categoryId, PostDto postDto)
        {
            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (authorIdRes == Guid.Empty) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var category = await _postService.CreatePost(categoryId, postDto, authorIdRes);

            if (category == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.POST_CREATE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_CREATED, category));
        }

        // Api to publish a post
        /*
            <summary>
                Publish a specific post by its ID
            </summary>
            <param name="postId">The ID of the post</param>
            <returns>Returns API Response containing Success, Status Code and Published Message</returns>
                <remarks>
                    <para>Note: Only the author of the post can publish it.</para>
                </remarks>
         */
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPatch("{postId}/publish")]
        public async Task<ActionResult<ApiResponse>> Publish(int postId)
        {
            Guid authorId = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (authorId == Guid.Empty) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var res = await _postService.PublishPost(postId, authorId);

            if (res.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (res.Equals("UnAuthorized")) return Conflict(new ApiResponse(false, 409, ResponseMessages.PUBLISH_CONFLICT));
            if (res.Equals("AlreadyPublished")) return Conflict(new ApiResponse(false, 409, ResponseMessages.PUBLISHED_POST_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_PUBLISHED));
        }


        // API to unpublish a post
        /* 
            <summary>
                Unpublish a specific post by its ID
            </summary>
            <param name="postId">The ID of the post</param>
            <returns>Returns API Response containing Success, Status Code and Unpublished Message</returns>
                <remarks>
                    <para>Note: Only the author of the post can unpublish it.</para>
                </remarks>
         */
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPatch("{postId}/unpublish")]
        public async Task<ActionResult<ApiResponse>> Unpublish(int postId)
        {
            Guid authorId = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (authorId == Guid.Empty) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var res = await _postService.UnpublishPost(postId, authorId);

            if (res.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (res.Equals("UnAuthorized")) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNPUBLISH_CONFLICT));
            if (res.Equals("NotPublishedYet")) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNPUBLISHED_POST_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_UNPUBLISHED));
        }


        // API to delete a Post
        /* 
            <summary>
                Delete a specific post by its ID
            </summary>
            <param name="postId">The ID of the post</param>
            <returns>Returns API Response containing Success, Status Code and Deleted Message</returns>
                <remarks>
                    <para>Note: Only the author of the post can delete it.</para>
                </remarks>
         */
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpDelete("{postId}")]
        public async Task<ActionResult<ApiResponse>> Delete(int postId)
        {
            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (authorIdRes == Guid.Empty)
            {
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            }

            var result = await _postService.DeletePost(postId, authorIdRes);

            if (result.Equals("NoPostFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_POST));
            if (result.Equals("InvalidAuthor")) return Conflict(new ApiResponse(false, 409, ResponseMessages.POST_DELETE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.POST_DELETE));
        }
    }
}
