using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;

namespace Blog_Application.Services
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> Register(RegisterDto registerDto);
        Task<LoginResponseDto> Login(LoginDto loginDto);
    }
}
