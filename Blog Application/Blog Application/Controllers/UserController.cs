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
        [HttpPost("subscribe/{authorId}")]
        public async Task<ActionResult<ApiResponse>> Subscribe(Guid authorId)
        {
            var authorIdRes = HelperFunctions.GetGuid(authorId.ToString() ?? "");
            if (authorIdRes == Guid.Empty) 
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_AUTHOR));

            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (userIdRes == Guid.Empty) 
                return Unauthorized(new ApiResponse(false, 401, ResponseMessages.LOGIN_TO_INTERACT));

            if (userIdRes == authorId)
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.SUBSCRIBE_SELF_CONFLICT));

            var result = await _userService.Subscribe(userIdRes, authorId);

            if (result == SubscribeResponse.InvalidAuthor) return StatusCode(403, new ApiResponse(false, 403, ResponseMessages.INVALID_SUBSCRIBE));
            if (result == SubscribeResponse.AlreadySubscribed) return BadRequest(new ApiResponse(false, 400, ResponseMessages.SUBSCRIBE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.SUBSCRIBE_SUCCESS));
        }

        // API to unsubscribe from an Author
        [HttpDelete("unsubscribe/{authorId}")]
        public async Task<ActionResult<ApiResponse>> Unsubscribe(Guid authorId)
        {
            var authorIdRes = HelperFunctions.GetGuid(authorId.ToString() ?? "");
            if (authorIdRes == Guid.Empty)
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_AUTHOR));

            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (userIdRes == Guid.Empty) 
                return Unauthorized(new ApiResponse(false, 401, ResponseMessages.LOGIN_TO_INTERACT));

            var result = await _userService.Unsubscribe(userIdRes, authorId);

            if (result == SubscribeResponse.InvalidAuthor) return StatusCode(403, new ApiResponse(false, 403, ResponseMessages.INVALID_AUTHOR));
            if (result == SubscribeResponse.NotYetSubscribed) return Conflict(new ApiResponse(false, 409, ResponseMessages.UNSUBSCRIBE_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.UNSUBSCRIBE_SUCCESS));
        }

        // Getting the subscribers of the Author (Anyone can see the subscribers of an Author)
        [HttpGet("subscribers/{authorId}")]
        public async Task<ActionResult<ApiResponse>> Subscribers(Guid authorId)
        {
            var authorIdRes = HelperFunctions.GetGuid(authorId.ToString() ?? "");
            if (authorIdRes == Guid.Empty)
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_AUTHOR));

            var subscribers = await _userService.GetSubscribers(authorId);

            if (subscribers == null) return Unauthorized(new ApiResponse(true, 200, ResponseMessages.SUBSCRIBER_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.SUBSCRIBERS_FETCHED, subscribers));
        }

        // Getting the subscriptions of the User (Only the User can see his subscriptions)
        [HttpGet("subscriptions/{userId}")]
        public async Task<ActionResult<ApiResponse>> Subscriptions(Guid userId)
        {
            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (userIdRes == Guid.Empty || userIdRes != userId) return Unauthorized(new ApiResponse(false, 401, ResponseMessages.SUBSCRIPTION_ACCESS_CONFLICT));

            var subscriptions = await _userService.GetSubscriptions(userId);

            if (!subscriptions.Any()) return Ok(new ApiResponse(true, 200, ResponseMessages.SUBSCRIPTION_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.SUBSCRIPTION_FETCHED, subscriptions));
        }
    }
}
