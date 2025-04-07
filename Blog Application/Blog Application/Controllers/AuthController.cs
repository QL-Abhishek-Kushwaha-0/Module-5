using Blog_Application.DTO.RequestDTOs;
using Microsoft.AspNetCore.Mvc;
using Blog_Application.Utils;
using Blog_Application.Services;
using Blog_Application.Resources;

namespace Blog_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

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


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
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
