using Blog_Application.Data;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog_Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Author)
                .Select(c => new CategoryResponseDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    AuthorName = c.Author.Name
                })
                .ToListAsync();

            return categories;
        }

        public async Task<CategoryResponseDto> GetCategoryById(int categoryId)
        {
            var categoryRes = await _context.Categories.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == categoryId);

            if (categoryRes == null) return null;

            var category = new CategoryResponseDto
            {
                CategoryId = categoryId,
                CategoryName = categoryRes.Name,
                AuthorName = categoryRes.Author.Name
            };

            return category;
        }
        public async Task<Category> CreateCategory(CategoryDto categoryDto, Guid authorId)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name.Equals(categoryDto.Name));

            if (existingCategory != null) return null;

            var newCategory = new Category { Name = categoryDto.Name, AuthorId = authorId };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return newCategory;
        }


        public async Task<CategoryResponseDto> UpdateCategory(CategoryDto categoryDto, int categoryId, Guid authorId)
        {
            var category = await _context.Categories.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null) return null;
            if (category.AuthorId != authorId) return null;

            category.Name = categoryDto.Name;

            await _context.SaveChangesAsync();

            var categoryRes = new CategoryResponseDto
            {
                CategoryId = categoryId,
                CategoryName = categoryDto.Name,
                AuthorName = category.Author.Name
            };

            return categoryRes;
        }

        public async Task<string> DeleteCategory(int categoryId, Guid authorId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null) return "NoCategoryFound";

            if (authorId != category.AuthorId) return "Unauthorized";

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return "Success";
        }
    }
}
