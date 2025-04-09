using System;
using System.Diagnostics;
using System.Text.Json;
using Blog_Application.DTO.LoggingDTOs;
using Blog_Application.Models.Entities;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Blog_Application.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var requestDetails = await GetRequestDetails(context);          // Fetches the details of Request 

            var originalResponseBodyStream = context.Response.Body;         // Stores the original Response Body
            await using var responseBodyMemoryStream = new MemoryStream();
            context.Response.Body = responseBodyMemoryStream;

            try
            {
                await _next(context);           // Calls the next Middleware (Here the Exception Handling Middleware) in the Application Execution Pipeline

                var responseDetails = await GetResponseDetails(context);        // Fetches the Response Details

                requestDetails.User = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous User (Not Logged In)";

                var executionTime = stopwatch.Elapsed.TotalSeconds;             // Calculates the Time taken by Api form Request to Response

                // Custom Logger (Serilog) to log the Information of API Request Response in a customized Format
                var logMessage = $@"
                    {requestDetails.Timestamp} | IP [{requestDetails.IpAddress}]
                    {requestDetails.Url} ({requestDetails.Method})
                    USER [{requestDetails.User}]
                    REQUEST_HEADERS: {requestDetails.Headers}
                    REQUEST BODY: {requestDetails.Body}
                    RESPONSE_HEADERS: {responseDetails.Headers}
                    RESPONSE BODY: {responseDetails.Body}
                    STATUS_CODE ~ {responseDetails.StatusCode} | EXECUTION_TIME [{executionTime} Seconds]";

                // Logging based on Success or failure
                if (responseDetails.StatusCode == 200)
                {
                    Log.Information(logMessage);
                }
                else
                {
                    Log.Error(logMessage);
                }

                responseBodyMemoryStream.Seek(0, SeekOrigin.Begin);
                // Resetting the Response body to the original one
                // THis is required as we reads the response using a "MemoryStream" (Memory stream allows to read the stream multiple times)
                // In case we send the response without rewiting the original one then the client would get a Empty Response body
                await responseBodyMemoryStream.CopyToAsync(originalResponseBodyStream);
            }
            finally
            {
                context.Response.Body = originalResponseBodyStream;
            }
        }


        // Method to fetch the Required Request details and store it in Request DTO
        private async Task<RequestDetails> GetRequestDetails(HttpContext context)
        {
            var request = context.Request;
            string requestBody = "None";

            // Method to read the Request Body in case of POST, PATCH and PUT request
            if (request.ContentLength > 0)
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                request.Body.Position = 0;
            }

            return new RequestDetails
            {
                Timestamp = DateTime.UtcNow.ToLocalTime().ToString("hh:mm:ss tt (UTC)"),
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                Method = request.Method,
                Url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}",
                User = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name : "Anonymous User (Not Logged In)",
                Headers = JsonSerializer.Serialize(request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
                Body = requestBody
            };
        }


        // MEthod to fetch the Response Details and store it in Response DTO
        private async Task<ResponseDetails> GetResponseDetails(HttpContext context)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            return new ResponseDetails
            {
                StatusCode = context.Response.StatusCode,
                Headers = JsonSerializer.Serialize(context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())),
                Body = responseBody
            };
        }
    }
}
