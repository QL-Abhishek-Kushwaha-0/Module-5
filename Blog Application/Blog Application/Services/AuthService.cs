using Blog_Application.Data;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Models.Entities;
using Blog_Application.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Blog_Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;
        public AuthService(MongoDbContext context, IConfiguration config, EmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }
        public async Task<RegisterResponseDto> Register(RegisterDto registerDto)
        {
            var existingUser = await _context.Users.Find(u => u.Email == registerDto.Email).AnyAsync();

            if (existingUser) return null;

            var newUser = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = PasswordHasher.HashPassword(registerDto.Password),
                Username = registerDto.Username,
                Role = registerDto.Role,
            };

            await _context.Users.InsertOneAsync(newUser);

            await _emailService.SendMail(newUser.Email, newUser.Name);

            return new RegisterResponseDto { Name = newUser.Name, Email = newUser.Email, Username = newUser.Username, Role = newUser.Role.ToString() };
        }
        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var existingUser = await _context.Users.Find(u => u.Email == loginDto.Email).FirstOrDefaultAsync();

            if (existingUser == null) return null;

            if (!PasswordHasher.VerifyPassword(loginDto.Password, existingUser.Password)) return null;

            string userId = Convert.ToString(existingUser.Id);

            var token = JwtTokenGenerator.GenerateJwtToken(_config, userId, existingUser.Name, existingUser.Email, existingUser.Role);

            return new LoginResponseDto { Name = existingUser.Name, Email = existingUser.Email, Username = existingUser.Username, AuthToken = token };
        }
    }
}
