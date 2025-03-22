using System.Diagnostics;
using System.Text.Json;
using Serilog.Core;
using Serilog.Events;

namespace Blog_Application.Logging
{
    public class HttpContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return;

            var request = context.Request;
            var response = context.Response;

            var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            var requestHeaders = JsonSerializer.Serialize(request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
            var responseHeaders = JsonSerializer.Serialize(response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
            var userName = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous User [Not Logged In]";

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RemoteIpAddress", remoteIp));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ApiUrl", fullUrl));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("HttpMethod", request.Method));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestHeaders", requestHeaders));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ResponseHeaders", responseHeaders));
            
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("StatusCode", response.StatusCode.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Username", userName));

            string requestBody = "None";
            // Read and log the request body properly
            if (request.Method == HttpMethods.Post || request.Method == HttpMethods.Patch || request.Method == HttpMethods.Put)
            {
                request.EnableBuffering(); // Allow multiple reads

                if (request.Body.CanRead && request.Body.Length > 0)
                {
                    request.Body.Position = 0;
                    using var reader = new StreamReader(request.Body, leaveOpen: true);
                    requestBody = reader.ReadToEnd();
                    request.Body.Position = 0; // Reset stream position for further processing
                }
            }

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestBody", requestBody));
        }
    }
}
