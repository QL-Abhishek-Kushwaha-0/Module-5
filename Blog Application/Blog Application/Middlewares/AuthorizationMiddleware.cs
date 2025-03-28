using System.Net;

namespace Blog_Application.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        { 
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                await context.Response.WriteAsJsonAsync(new ApiResponse(false, 403, "Only Authors are allowed to Create Category!!!!"));
            }
        }
    }
}
