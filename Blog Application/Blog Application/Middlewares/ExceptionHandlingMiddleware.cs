using System.Net;
using System.Text.Json;
using Serilog;

namespace Blog_Application.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);  // Will the the api controller as it is next in pipeline
            }
            catch (Exception ex)
            {
                Log.Error($"Exception: {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;  
            context.Response.ContentType = "application/json";

            var response = new ApiResponse(false, context.Response.StatusCode, ex.Message);         // Store the response in a Response Utility -> ApiResponse

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
