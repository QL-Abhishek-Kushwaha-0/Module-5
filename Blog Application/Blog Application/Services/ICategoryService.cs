using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Models.Entities;

namespace Blog_Application.Services
{
    public interface ICategoryService
    {
        Task<Category> CreateCategory(CategoryDto category, Guid authorId);
        Task<string> DeleteCategory(int categoryId, Guid authorId);
        Task<List<CategoryResponseDto>> GetAllCategories();
    }
}
