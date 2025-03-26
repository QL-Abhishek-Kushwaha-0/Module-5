using Blog_Application.Data;
using Blog_Application.DTO;
using Blog_Application.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Blog_Application.Middlewares;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Blog_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser != null)
            {
                return Conflict(new { Response = new ApiResponse(false, 409, "User already exists!!!!") });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var newUser = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = hashedPassword,
                Username = registerDto.Username,
                Role = registerDto.Role
            };

            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                response = new ApiResponse(true, 200, "User Registered Successfully!!!!"),
                data = new { newUser.Name, newUser.Email, newUser.Username }
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (existingUser == null)
            {
                return NotFound(new { response = new ApiResponse(false, 404, "No User Found!!!") });
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, existingUser.Password))
            {
                return Unauthorized(new { response = new ApiResponse(false, 401, "Unauthorized Access!!!") });
            }

            var token = GenerateJwtToken(existingUser.Name, existingUser.Email, existingUser.Role);
            return Ok(new
            {
                response = new ApiResponse(true, 200, "Logged In Successfully....."),
                JwtToken = token,
                data = new { existingUser.Name, existingUser.Email, existingUser.Username }
            });
        }


        // Method to Generate the JWT Token
        private string GenerateJwtToken(string name, string email, UserRole role)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            string roleStr = Convert.ToString(role) ?? "";
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, name),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, roleStr)
            };

            var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"])),
                    signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
