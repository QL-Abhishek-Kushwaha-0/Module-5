using Blog_Application.DTO.RequestDTOs;
using Microsoft.AspNetCore.Mvc;
using Blog_Application.Middlewares;
using Blog_Application.Services;

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
                return Conflict(new ApiResponse(false, 409, "User already exists!!!"));
            }

            return Ok(new ApiResponse(true, 200, "User Registered Successfully!!!!", registerResponse));
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _authService.Login(loginDto);

            if (loginResponse == null)
            {
                return NotFound(new ApiResponse(false, 404, "Invalid Email or Password!!!!"));
            }

            return Ok(new ApiResponse(true, 200, "Logged In Successfully....", loginResponse));
        }
    }
}
