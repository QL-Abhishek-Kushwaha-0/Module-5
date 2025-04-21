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
using Microsoft.AspNetCore.Mvc;

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

        // Seperated the Info logs and Error logs in different files

        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
            .WriteTo.File("Logs/Information-Logs/info-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
        )

        .WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(e =>
                e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Fatal)
            .WriteTo.File("Logs/Error-Logs/error-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
        );
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


// Overrides the default Model Validation (Invalid Model State) Error Fromat and Customized in Custom ApiResponse Format
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            var customResponse = new ApiResponse(false, 400, "Validation Failed!!", errors);

            return new BadRequestObjectResult(customResponse);
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddSingleton<S3Service>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<EmailService>();

//builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<RequestResponseLoggingMiddleware>();          // Middleware for Logging the Request Response  
app.UseMiddleware<AuthorizationMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();               // Middleware for Handling the Exceptions

app.UseHttpsRedirection();

app.UseAuthentication();    // Used to Enable the JWT Authentication
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "This is a Blog Application....");

app.Run();
