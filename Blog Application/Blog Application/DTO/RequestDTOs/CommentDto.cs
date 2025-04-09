using System.ComponentModel.DataAnnotations;
using Blog_Application.Resources;

namespace Blog_Application.DTO.RequestDTOs
{
    public class CommentDto
    {
        [MinLength(3, ErrorMessage = ResponseMessages.MIN_LENGTH)]
        public string Content { get; set; }
    }
}
