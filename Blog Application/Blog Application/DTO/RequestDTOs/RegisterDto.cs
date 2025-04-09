using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Blog_Application.Models.Entities;
using Blog_Application.Resources;

namespace Blog_Application.DTO.RequestDTOs
{
    public class RegisterDto
    {
        [MinLength(3, ErrorMessage =ResponseMessages.MIN_LENGTH)]
        [MaxLength(50, ErrorMessage =ResponseMessages.MAX_NAME_LENGTH)]
        public required string Name { get; set; }

        [EmailAddress(ErrorMessage =ResponseMessages.INVALID_EMAIL)]
        public required string Email { get; set; }

        [MinLength(8, ErrorMessage =ResponseMessages.INVALID_PASSWORD)]
        public required string Password { get; set; }
        public string Username { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; } = UserRole.Viewer;
    }
}
