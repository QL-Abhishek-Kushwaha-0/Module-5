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
                //Log.Error($"Exception: {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            if (context.Response.HasStarted)
            {
                Log.Warning("The response has already started, cannot modify the status code or write the error response.");
                return;
            }

            context.Response.Clear();  // Clear any partially written response
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;  
            context.Response.ContentType = "application/json";

            var response = new ApiResponse(false, context.Response.StatusCode, ex.Message);         // Store the response in a Response Utility -> ApiResponse

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
