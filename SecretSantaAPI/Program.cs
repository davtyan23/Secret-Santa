using Business;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext, repositories, and other services
builder.Services.AddDbContext<SecretSantaContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register EmailSender as a Singleton
builder.Services.AddSingleton<EmailSender>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Run the application
app.Run();
