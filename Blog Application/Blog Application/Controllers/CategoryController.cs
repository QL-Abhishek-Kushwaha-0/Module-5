using System.Security.Claims;
using System.Threading.Tasks;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.Helper;
using Blog_Application.Middlewares;
using Blog_Application.Models.Entities;
using Blog_Application.Resources;
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
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            var categories = await _categoryService.GetAllCategories();

            if (!categories.Any()) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORIES));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORIES_FETCHED, categories));
        }


        [HttpGet("{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Get(int categoryId)
        {
            var category = await _categoryService.GetCategoryById(categoryId);

            if (category == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORY));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORY_FETCHED, category));
        }


        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] CategoryDto categoryDto)
        {

            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            
            if (authorIdRes == Guid.Empty)
            {
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            }
            Guid authorId = authorIdRes;

            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;

            var newCategory = await _categoryService.CreateCategory(categoryDto, authorId);

            if (newCategory == null) return Conflict(new ApiResponse(false, 409, ResponseMessages.CATEGORY_EXISTS));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORY_CREATED, new { category_name = newCategory.Name, author_name = authorName }));
        }


        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPut("{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Update([FromBody] CategoryDto categoryDto, int categoryId)
        {
            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (authorIdRes == Guid.Empty) return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            Guid authorId = authorIdRes;

            var updatedCategory = await _categoryService.UpdateCategory(categoryDto, categoryId, authorId);

            if (updatedCategory == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORY));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORY_UPDATED, updatedCategory));
        }


        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpDelete("{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Delete(int categoryId)
        {
            var authorIdRes = HelperFunctions.GetGuid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");
            if (authorIdRes == Guid.Empty)
            {
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            }
            Guid authorId = authorIdRes;

            var res = await _categoryService.DeleteCategory(categoryId, authorId);

            if (res.Equals("NoCategoryFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORY));
            if (res.Equals("Unauthorized")) return Conflict(new ApiResponse(false, 409, ResponseMessages.CATEGORY_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORY_DELETE));
        }
    }
}
