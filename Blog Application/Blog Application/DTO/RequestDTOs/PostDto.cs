using System.ComponentModel.DataAnnotations;
using Blog_Application.Resources;

namespace Blog_Application.DTO.RequestDTOs
{
    public class PostDto
    {
        [MinLength(3, ErrorMessage = ResponseMessages.MIN_LENGTH)]
        public string Title { get; set; }

        [MinLength(3, ErrorMessage = ResponseMessages.MIN_LENGTH)]
        public string Description { get; set; }

        [Url(ErrorMessage = ResponseMessages.INVALID_URL)]
        public string? ImageUrl { get; set; }
    }
}
