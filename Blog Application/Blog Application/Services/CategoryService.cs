using Blog_Application.Data;
using Blog_Application.DTO;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.DTO.ResponseDTOs;
using Blog_Application.Helper;
using Blog_Application.Models.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace Blog_Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly MongoDbContext _context;

        public CategoryService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategories()
        {
            // Aggregation Based Fetching of Category and Author

            var categoryRes = await _context.Categories.Aggregate()
                .Lookup<Category, User, LookupClasses.CategoryWithAuthor>(
                _context.Users,
                c => c.AuthorId,
                u => u.Id,
                res => res.Author
                )
                .Project(c => new CategoryResponseDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    AuthorName = c.Author.FirstOrDefault().Name
                }).ToListAsync();

            return categoryRes ?? new List<CategoryResponseDto>();
        }
        public async Task<CategoryResponseDto> GetCategoryById(string categoryId)
        {
            var categoryRes = await _context.Categories.Find(c => c.Id == categoryId).FirstOrDefaultAsync();
            if (categoryRes == null) return null;

            var author = await _context.Users.Find(u => u.Id == categoryRes.AuthorId).FirstOrDefaultAsync();

            var category = new CategoryResponseDto
            {
                CategoryId = categoryId,
                CategoryName = categoryRes.Name,
                AuthorName = author.Name
            };

            return category;
        }
        public async Task<Category> CreateCategory(CategoryDto categoryDto, string authorId)
        {
            var existingCategory = await _context.Categories
                .Find(c => c.Name.ToLower().Equals(categoryDto.Name.ToLower()))
                .AnyAsync();

            if (existingCategory) return null;

            var newCategory = new Category { Name = categoryDto.Name, AuthorId = authorId };

            await _context.Categories.InsertOneAsync(newCategory);

            return newCategory;
        }
        public async Task<CategoryResponseDto> UpdateCategory(CategoryDto categoryDto, string categoryId, string authorId)
        {
            var category = await _context.Categories.Find(c => c.Id == categoryId).FirstOrDefaultAsync();
            if (category == null || category.AuthorId != authorId) return null;

            var author = await _context.Users.Find(a => a.Id == authorId).FirstOrDefaultAsync();

            // Update the Category Name
            await _context.Categories.UpdateOneAsync(
                c => c.Id == categoryId,
                Builders<Category>.Update.Set(c => c.Name, categoryDto.Name)
            );

            return new CategoryResponseDto { CategoryId = categoryId, CategoryName = categoryDto.Name, AuthorName = author.Name };
        }

        public async Task<string> DeleteCategory(string categoryId, string authorId)
        {
            var category = await _context.Categories.Find(c => c.Id == categoryId).FirstOrDefaultAsync();

            if (category == null) return "NoCategoryFound";

            var postExists = await _context.Posts.Find(c => c.CategoryId == categoryId).AnyAsync();

            if (postExists) return "PostExists";

            if (authorId != category.AuthorId) return "Unauthorized";

            await _context.Categories.DeleteOneAsync(Builders<Category>.Filter.Eq(c => c.Id, category.Id));

            return "Success";
        }
    }
}
