using Blog_Application.Data;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Models.Entities;
using Blog_Application.Utils;
using Microsoft.EntityFrameworkCore;

namespace Blog_Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        public AuthService(ApplicationDbContext context, IConfiguration config, EmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }
        public async Task<RegisterResponseDto> Register(RegisterDto registerDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser != null) return null;

            var newUser = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = PasswordHasher.HashPassword(registerDto.Password),
                Username = registerDto.Username,
                Role = registerDto.Role,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            await _emailService.SendMail(newUser.Email, newUser.Name);

            return new RegisterResponseDto { Name = newUser.Name, Email = newUser.Email, Username = newUser.Username, Role = newUser.Role.ToString() };
        }
        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (existingUser == null) return null;

            if (!PasswordHasher.VerifyPassword(loginDto.Password, existingUser.Password)) return null;

            string userId = Convert.ToString(existingUser.Id);

            var token = JwtTokenGenerator.GenerateJwtToken(_config, userId, existingUser.Name, existingUser.Email, existingUser.Role);

            return new LoginResponseDto { Name = existingUser.Name, Email = existingUser.Email, Username = existingUser.Username, AuthToken = token };
        }
    }
}
