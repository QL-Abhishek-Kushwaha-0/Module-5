using Blog_Application.DTO.RequestDTOs;
using Microsoft.AspNetCore.Mvc;
using Blog_Application.Utils;
using Blog_Application.Services;
using Blog_Application.Resources;

namespace Blog_Application.Controllers
{
    [ApiController]     // This automatically validates the Incoming Model / Dto  & also binds Source inferences (No need to use [FromBody] or [FromQuery] attributes
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // API to Register a new user
        /*
            <summary>
                Register a new user
            </summary>
            <param name="registerDto">The registration details</param>
            <returns>Returns a success message and the registered user details</returns>
         */
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var registerResponse = await _authService.Register(registerDto);

            if (registerResponse == null)
            {
                return Conflict(new ApiResponse(false, 409, ResponseMessages.USER_EXISTS));
            }

            return Ok(new ApiResponse(true, 200, ResponseMessages.USER_REGISTERED, registerResponse));
        }

        // API to Login a user
        /*
            <summary>
                Login a user
            </summary>
            <param name="loginDto">The login details</param>
            <returns>Returns a success message and the logged-in user details</returns>
         */
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var loginResponse = await _authService.Login(loginDto);

            if (loginResponse == null)
            {
                return NotFound(new ApiResponse(false, 404, ResponseMessages.INVALID_CREDENTIALS));
            }

            return Ok(new ApiResponse(true, 200, ResponseMessages.LOGIN_SUCCESS, loginResponse));
        }
    }
}
