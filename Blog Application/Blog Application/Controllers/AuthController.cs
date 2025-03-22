using Blog_Application.Middlewares;
using Blog_Application.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Application.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static Dictionary<int, string> Blogs = new()
        {
            { 1, "Understanding .NET Middleware" },
            { 2, "Building REST APIs with ASP.NET Core" }
        };

        // Example  API for testing the Logging and Exception Handling   
        [HttpGet("{id}")]
        public IActionResult GetBlogById(int id)
        {
            if (!Blogs.ContainsKey(id))
            {
                throw new GlobalException(); // This will be caught by ExceptionMiddleware
            }
            
            return Ok(new
            {
                response = new ApiResponse(true, 200, "Blog Fetched Successfully!!!"),
                data = new { id, title = Blogs[id] }
            });

        }
    }
}
