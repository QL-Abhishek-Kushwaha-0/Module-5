using System.Security.Claims;
using System.Threading.Tasks;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.Middlewares;
using Blog_Application.Models.Entities;
using Blog_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Application.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> Get()
        {
            var categories = await _categoryService.GetAllCategories();

            if (!categories.Any()) return NotFound(new ApiResponse(false, 404, "No Categories Found"));

            return Ok(new ApiResponse(true, 200, "Categories Fetched Successfully!!!", categories));
        }

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] CategoryDto categoryDto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            Guid userId;
            if (!Guid.TryParse(userIdString, out userId))
            {
                return BadRequest(new ApiResponse(false, 400, "Invalid User!!!"));
            }

            var newCategory = await _categoryService.CreateCategory(categoryDto, userId);

            if (newCategory == null) return Conflict(new ApiResponse(false, 409, "Category already exists."));

            return Ok(new ApiResponse(true, 200, "Category created Successfully...", new { category_name = newCategory.Name, author_name = userName }));
        }

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpDelete("{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Delete(int categoryId)
        {
            Guid authorId;
            if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out authorId))
            {
                return BadRequest(new ApiResponse(false, 400, "Invalid Author!!!!"));
            }

            var res = await _categoryService.DeleteCategory(categoryId, authorId);

            if (res.Equals("NoCategoryFound")) return NotFound(new ApiResponse(false, 404, "No such Category Found!!!"));
            if (res.Equals("Unauthorized")) return Conflict(new ApiResponse(false, 409, "Cannot delete Categories created by other Author!!!"));

            return Ok(new ApiResponse(true, 200, "Category Deleted Successfully...."));
        }
    }
}
