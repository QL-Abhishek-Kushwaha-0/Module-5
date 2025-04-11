using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Models.Entities;

namespace Blog_Application.Services
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(CategoryDto category, string authorId);
        Task<string> DeleteCategory(string categoryId, string authorId);
        Task<CategoryResponseDto> GetCategoryById(string categoryId);
        Task<CategoryResponseDto> UpdateCategory(CategoryDto categoryDto, string categoryId, string authorId);
        Task<List<CategoryResponseDto>> GetAllCategories();
    }
}
