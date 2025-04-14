using System.ComponentModel.DataAnnotations;
using Blog_Application.Resources;

namespace Blog_Application.DTO.RequestDTOs
{
    public class CategoryDto
    {
        [MinLength(3, ErrorMessage = ResponseMessages.MIN_LENGTH)]
        [MaxLength(20, ErrorMessage = ResponseMessages.MAX_LENGTH)]
        public required string Name { get; set; }
    }
}
