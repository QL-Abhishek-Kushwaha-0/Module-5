using System.Security.Claims;
using Blog_Application.DTO.RequestDTOs;
using Blog_Application.Helper;
using Blog_Application.Utils;
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


        // API to get all categories
        /*
            <summary>
                Get all categories
            </summary>
            <param>None</param
            <returns>Returns API Response containing Success, Status Code , Message and Categories data</returns>
         */
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            var categories = await _categoryService.GetAllCategories();

            if (!categories.Any()) return Ok(new ApiResponse(true, 200, ResponseMessages.NO_CATEGORY_CREATED));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORIES_FETCHED, categories));
        }

        // API to get a specific category by ID
        /*
            <summary>
                Get a specific category by its ID
            </summary>
            <param name="categoryId">The ID of the category</param>
            <returns>Returns API Response containing Success, Status Code , Message and Category data</returns>
         */
        [HttpGet("{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Get(string categoryId)
        {
            var category = await _categoryService.GetCategoryById(categoryId);

            if (category == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORY));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORY_FETCHED, category));
        }


        // API to create a new category
        /*
            <summary>
                Create a new category
            </summary>
            <param name="categoryDto">The details of the category to be created</param>
            <returns>Returns API Response containing Success, Status Code , Message and Categories data</returns>
         */

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create(CategoryDto categoryDto)
        {

            var authorIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authorIdRes == null)
            {
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            }

            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;

            var newCategory = await _categoryService.CreateCategory(categoryDto, authorIdRes);

            if (newCategory == null) return Conflict(new ApiResponse(false, 409, ResponseMessages.CATEGORY_EXISTS));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORY_CREATED, new { category_name = newCategory.Name, author_name = authorName }));
        }

        // API to update an existing category
        /* 
            <summary>
                Update an existing category
            </summary>
            <param name="categoryDto">The updated details of the category</param>
            <param name="categoryId">The ID of the category to be updated</param>
            <returns>Returns API Response containing Success, Status Code , Message and updated Category data</returns>
                <remarks>
                    The category ID should be passed in the URL as a parameter.
                    The updated details of the category should be passed in the request body.
                </remarks>
         */

        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpPut("{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Update(CategoryDto categoryDto, string categoryId)
        {

            var authorIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (authorIdRes == null)
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));

            var updatedCategory = await _categoryService.UpdateCategory(categoryDto, categoryId, authorIdRes);

            if (updatedCategory == null) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORY));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORY_UPDATED, updatedCategory));
        }

        // API to delte Category
        /* 
            <summary>
                Delete a category
            </summary>
            <param name="categoryId">The ID of the category to be deleted</param>
            <returns>Returns API Response containing Success, Status Code, Message</returns>
                <remarks>
                    The category ID should be passed in the URL as a parameter.
                </remarks>
         */
        [Authorize(Roles = nameof(UserRole.Author))]
        [HttpDelete("{categoryId}")]
        public async Task<ActionResult<ApiResponse>> Delete(string categoryId)
        {
            var authorIdRes = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (authorIdRes == null)
            {
                return BadRequest(new ApiResponse(false, 400, ResponseMessages.INVALID_GUID));
            }

            var res = await _categoryService.DeleteCategory(categoryId, authorIdRes);

            if (res.Equals("NoCategoryFound")) return NotFound(new ApiResponse(false, 404, ResponseMessages.NO_CATEGORY));
            if (res.Equals("PostExists")) return Conflict(new ApiResponse(false, 409, ResponseMessages.HAVE_POSTS_CONFLICT));
            if (res.Equals("Unauthorized")) return Conflict(new ApiResponse(false, 409, ResponseMessages.CATEGORY_CONFLICT));

            return Ok(new ApiResponse(true, 200, ResponseMessages.CATEGORY_DELETE));
        }
    }
}
