using System.Security.Claims;
using Blog_Application.Enums;
using Blog_Application.Helper;
using Blog_Application.Utils;
using Blog_Application.Resources;
using Blog_Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Application.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // API to Subscribe to an Author
        /* 
            <summary>
                Subscribe to an Author
            </summary>
            <param name="authorId">The ID of the author to subscribe to</param>
            <returns>Returns a success message if the subscription is successful</returns>
         */
        [HttpPost("subscribe/{authorId}")]
        public async Task<ActionResult<ApiResponse>> Subscribe(string authorId)
        {
            if (authorId == null) 
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_AUTHOR));

            var userIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdRes == null) 
                return Unauthorized(new ApiResponse(false, 401, ResponseMessages.LOGIN_TO_INTERACT));

            if (userIdRes == authorId)
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.SUBSCRIBE_SELF_CONFLICT));

            var result = await _userService.Subscribe(userIdRes, authorId);

            if (result == SubscribeResponse.InvalidAuthor) return StatusCode(403, new ApiResponse(false, 403, ResponseMessages.INVALID_SUBSCRIBE));
            if (result == SubscribeResponse.AlreadySubscribed) return BadRequest(new ApiResponse(false, 400, ResponseMessages.SUBSCRIBE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.SUBSCRIBE_SUCCESS));
        }

     
        // API to Unsubscribe to an Author
        /*
            <summary>
                Unsubscribe from an Author
            </summary>
            <param name="authorId">The ID of the author to unsubscribe from</param>
            <returns>Returns a success message if the unsubscription is successful</returns>
         */
        [HttpDelete("unsubscribe/{authorId}")]
        public async Task<ActionResult<ApiResponse>> Unsubscribe(string authorId)
        {
            if (authorId == null)
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_AUTHOR));

            var userIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdRes == null)
                return Unauthorized(new ApiResponse(false, 401, ResponseMessages.LOGIN_TO_INTERACT));

            var result = await _userService.Unsubscribe(userIdRes, authorId);

            if (result == SubscribeResponse.InvalidAuthor) return StatusCode(403, new ApiResponse(false, 403, ResponseMessages.INVALID_AUTHOR));
            if (result == SubscribeResponse.NotYetSubscribed) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNSUBSCRIBE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.UNSUBSCRIBE_SUCCESS));
        }

        // API to fetch the Subscribers of an Author
        /* 
            <summary>
                Get the subscribers of an Author
            </summary>
            <param name="authorId">The ID of the author whose subscribers are to be fetched</param>
            <returns>Returns a list of subscribers of the author</returns>
         */
        [HttpGet("subscribers/{authorId}")]
        public async Task<ActionResult<ApiResponse>> Subscribers(string authorId)
        {
            if (authorId == null)
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_AUTHOR));

            var subscribers = await _userService.GetSubscribers(authorId);

            if (subscribers == null) return Unauthorized(new ApiResponse(true, 200, ResponseMessages.SUBSCRIBER_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.SUBSCRIBERS_FETCHED, subscribers));
        }

        // API to fetch the subscriptions of a User
        /* 
            <summary>
                Get the subscriptions of a User
            </summary>
            <param name="userId">The ID of the user whose subscriptions are to be fetched</param>
            <returns>Returns a list of subscriptions of the user</returns>
            <remarks>
                    <para>Note: Only the user can see his subscriptions.</para>
            </remarks>
         */
        [HttpGet("subscriptions/{userId}")]
        public async Task<ActionResult<ApiResponse>> Subscriptions(string userId)
        {
            var userIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdRes == null || userIdRes != userId) return Unauthorized(new ApiResponse(false, 401, ResponseMessages.SUBSCRIPTION_ACCESS_CONFLICT));

            var subscriptions = await _userService.GetSubscriptions(userId);

            if (!subscriptions.Any()) return Ok(new ApiResponse(true, 200, ResponseMessages.SUBSCRIPTION_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.SUBSCRIPTION_FETCHED, subscriptions));
        }
    }
}
