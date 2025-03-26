using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Blog_Application.Models.Entities;

namespace Blog_Application.DTO
{
    public class RegisterDto
    {
        [MinLength(3, ErrorMessage ="Name length cannot be less than 3!!!!")]
        [MaxLength(50)]
        public required string Name { get; set; }

        [EmailAddress(ErrorMessage ="Invalid Email Format!!!")]
        public required string Email { get; set; }

        [MinLength(8, ErrorMessage ="Password must be at least of legnth = 8!!!")]
        public required string Password { get; set; }
        public string Username { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required UserRole Role { get; set; } = UserRole.Viewer;
    }
}
