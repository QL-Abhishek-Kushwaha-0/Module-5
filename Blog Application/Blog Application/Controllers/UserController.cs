using System.Security.Claims;
using Blog_Application.Enums;
using Blog_Application.Helper;
using Blog_Application.Middlewares;
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

        [HttpPost("subscribe/{authorId:guid}")]
        public async Task<ActionResult<ApiResponse>> Subscribe(Guid authorId)
        {
            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (userIdRes == Guid.Empty)
            {
                return Unauthorized(new ApiResponse(false, 401, "Login to Continue...."));
            }
            if(userIdRes == authorId)
                return BadRequest(new ApiResponse(false, 400, "You cannot subscribe to yourself"));

            var result = await _userService.Subscribe(userIdRes, authorId);

            if(result == SubscribeDto.InvalidAuthor) return StatusCode(403, new ApiResponse(false, 403, "Can only Subscribe to Authors!!"));
            if(result == SubscribeDto.AlreadySubscribed) return BadRequest(new ApiResponse(false, 400, "Already Subscribed to this Author!!"));

            return Ok(new ApiResponse(true, 200, "Subscribed Successfully...."));
        }

        [HttpDelete("unsubscribe/{authorId:guid}")]
        public async Task<ActionResult<ApiResponse>> Unsubscribe(Guid authorId)
        {
            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (userIdRes == Guid.Empty) return Unauthorized(new ApiResponse(false, 401, "Please Login to Continue!!!!"));

            var result = await _userService.Unsubscribe(userIdRes, authorId);

            if (result == SubscribeDto.InvalidAuthor) return StatusCode(403, new ApiResponse(false, 403, "Invalid Author!!!!"));
            if (result == SubscribeDto.NotYetSubscribed) return Conflict(new ApiResponse(false, 409, "You have not yet subscribed to this Author!!!"));

            return Ok(new ApiResponse(true, 200, "unsubscribed Successfully...."));
        }

        [HttpGet("subscribers/{authorId:guid}")]
        public async Task<ActionResult<ApiResponse>> Subscribers(Guid authorId)
        {
            var subscribers = await _userService.GetSubscribers(authorId);

            if (subscribers == null) return Unauthorized(new ApiResponse(true, 200, "Invalid Request!! Viewers cannot have subscribers!!!"));

            return Ok(new ApiResponse(true, 200, "Subscribers Fetched Successfully..", subscribers));
        }

        [HttpGet("subscriptions/{userId:guid}")]
        public async Task<ActionResult<ApiResponse>> Subscriptions(Guid userId)
        {
            var userIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

            if (userIdRes == Guid.Empty || userIdRes != userId) return Unauthorized(new ApiResponse(false, 401, "Cannot Access other user's subscriptions!!!"));

            var subscriptions = await _userService.GetSubscriptions(userId);

            if (!subscriptions.Any()) return Ok(new ApiResponse(true, 200, "You haven't Subscribed to any Author!!!"));

            return Ok(new ApiResponse(true, 200, "Subscription Fetched Successfully....", subscriptions));
        }
    }
}
