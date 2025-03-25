using Blog_Application.Middlewares;
using Serilog;
using Serilog.Events;
using Blog_Application.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Getting Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Database Connection
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(connectionString));

//Console.WriteLine($"Connection String: {connectionString}");

// Serilog Configuration
builder.Host.UseSerilog((context, services, config) =>
{
    config
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10);
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<RequestResponseLoggingMiddleware>();          // Middleware for Logging the Request Response  
app.UseMiddleware<ExceptionHandlingMiddleware>();               // Middleware for Handling the Exceptions

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Test route
app.MapGet("/", () => "This is a Blog Application....");

app.Run();
