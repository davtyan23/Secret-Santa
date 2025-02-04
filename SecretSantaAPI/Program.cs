using Business.Services;
using Business;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SecretSantaAPI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddMvcCore();
builder.Services.AddRazorPages(); // Add Razor Pages support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Swagger with JWT authentication support
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Authorization policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ParticipantPolicy", policy =>
        policy.RequireRole("Admin")); // Example role-based policy
});

// Configure Cookie and JWT Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "AuthToken"; // Name for the authentication cookie
        options.LoginPath = "/LoginPage/Login"; // Redirect to Login on unauthorized
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect on access denied
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Cookie expiration
        options.SlidingExpiration = true; // Refresh cookie expiration on use
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false; // Disable for dev; enable in prod
        options.SaveToken = true; // Save token in the request context
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Add database context and services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SecretSantaContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ILoggerAPI, LoggerAPI>();
builder.Services.AddOptions<SmtpOptions>().BindConfiguration(nameof(SmtpOptions));
builder.Services.AddScoped<SecretSantaService>();
// Register EmailSender as Singleton
builder.Services.AddSingleton<EmailSender>();

var app = builder.Build();

// Middleware pipeline
app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication(); // Enable Authentication Middleware
app.UseAuthorization(); // Enable Authorization Middleware

app.UseHttpsRedirection();
app.UseStaticFiles();

// Map Razor Pages with route constraints
app.MapRazorPages(); // Ensure this is present to enable Razor Pages routing

// Map Controllers
app.MapControllers();

// Add support for custom Razor Pages routes
app.MapFallbackToPage("/Groups/JoinGroup", "/Groups/JoinGroup"); // Ensure token-based routing works

app.Run();
