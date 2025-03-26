using System.ComponentModel.DataAnnotations;

namespace Blog_Application.DTO
{
    public class LoginDto
    {
        [EmailAddress(ErrorMessage ="Enter a Valid Email Address!!!!")]
        public required string Email { get; set; }

        public required string Password { get; set; }

    }
}
