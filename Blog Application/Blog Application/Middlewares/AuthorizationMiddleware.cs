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
            var endpoint = context.GetEndpoint();
            var hasAuthorizeAttribute = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;

            if (!hasAuthorizeAttribute || context.Response.HasStarted)
                return;

            // Run middleware logic only if the endpoint has [Authorize] attribute
            if (hasAuthorizeAttribute)
            {

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await context.Response.WriteAsJsonAsync(new ApiResponse(false, 401, "Please login first!!!!"));
                }

                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    await context.Response.WriteAsJsonAsync(new ApiResponse(false, 403, "You are not authorized to access this resource!!!!"));
                }
                await context.Response.WriteAsJsonAsync(new ApiResponse(false, 403, ResponseMessages.INVALID_AUTHOR));

            }
        }
    }
}
