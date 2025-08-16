using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RibbitReels.Data;
using RibbitReels.Data.Models;
using RibbitReels.Services.Interfaces;
using RibbitReels.Services.Implementations;
using System.Text;
using RibbitReels.Data.Configs;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<AzureBlobConfiguration>(options =>
{
    // connection string
    options.ConnectionString =
        Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ??
        builder.Configuration.GetConnectionString("AzureBlobStorage") ??
        string.Empty;

    // fallback to account name + key if no connection string
    options.AccountName =
        Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT") ??
        builder.Configuration["Azure:AccountName"] ??
        string.Empty;

    options.AccountKey =
        Environment.GetEnvironmentVariable("AZURE_STORAGE_KEY") ??
        builder.Configuration["Azure:AccountKey"] ??
        string.Empty;

    // Default container name
    options.ContainerName =
        builder.Configuration["Azure:ContainerName"] ??
        "videos";
});

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing");
var jwtIssuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing");
var jwtAudience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience is missing");

builder.Services.Configure<GoogleAuthConfiguration>(builder.Configuration.GetSection("GoogleAuth"));
builder.Services.Configure<TestUserOptions>(builder.Configuration.GetSection("Authentication:TestUser"));

var key = Encoding.UTF8.GetBytes(jwtKey);

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// CORS Setup
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
    policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"[Program.cs] ConnectionString = {connectionString}");

if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("DefaultConnection not found");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Services
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ILeafService, LeafService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserBranchAssignmentService, UserBranchAssignmentService>();
builder.Services.AddScoped<ILearningProgressService, LearningProgressService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddSingleton<AzureBlobRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
