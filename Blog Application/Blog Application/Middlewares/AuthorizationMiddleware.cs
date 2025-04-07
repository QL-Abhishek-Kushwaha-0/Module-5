using System.Net;
using Blog_Application.Resources;

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

            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                await context.Response.WriteAsJsonAsync(new ApiResponse(false, 401, ResponseMessages.INVALID_LOGIN));
            }

            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                await context.Response.WriteAsJsonAsync(new ApiResponse(false, 403, ResponseMessages.INVALID_AUTHOR));
            }
        }
    }
}
