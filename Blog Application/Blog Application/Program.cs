using Blog_Application.Middlewares;
using Serilog;
using Serilog.Events;
using Blog_Application.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blog_Application.Utils;
using Blog_Application.Services;

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

// Implementing JWT Auhtentication and Authorization

var jwtSettings = builder.Configuration.GetSection("JwtSettings");      // Getting JWT Settings
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

//Console.WriteLine($"{key} {jwtSettings["Key"]}");

// Registering the Authentication Service for JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();               // Middleware for Handling the Exceptions

app.UseStaticFiles();

app.UseMiddleware<AuthorizationMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();          // Middleware for Logging the Request Response  


app.UseHttpsRedirection();

app.UseAuthentication();    // Used to Enable the JWT Authentication
app.UseAuthorization();

app.MapControllers();

// Test route
app.MapGet("/", () => "This is a Blog Application....");

app.Run();
