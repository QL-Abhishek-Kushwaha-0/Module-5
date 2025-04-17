using System.ComponentModel.DataAnnotations;
using Blog_Application.Resources;

namespace Blog_Application.DTO.RequestDTOs
{
    public class LoginDto
    {
        [EmailAddress(ErrorMessage = ResponseMessages.INVALID_EMAIL)]
        public required string Email { get; set; }
        public required string Password { get; set; }

    }
}
