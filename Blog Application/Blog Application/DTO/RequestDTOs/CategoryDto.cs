using System.ComponentModel.DataAnnotations;

namespace Blog_Application.DTO.RequestDTOs
{
    public class CategoryDto
    {
        [MinLength(3, ErrorMessage = "Category name must be at least 3 characters long.")]
        [MaxLength(20, ErrorMessage = "Category name cannot exceed 20 characters.")]
        public required string Name { get; set; }
    }
}
