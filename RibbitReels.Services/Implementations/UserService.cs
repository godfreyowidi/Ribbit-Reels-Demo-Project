using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RibbitReels.Data;
using RibbitReels.Data.DTOs;
using RibbitReels.Data.Models;
using RibbitReels.Services.Interfaces;
using RibbitReels.Services.Shared;

public class UserService : IUserService
{
    private readonly AppDbContext _appDbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public UserService(AppDbContext appDbContext, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _appDbContext = appDbContext;
        _passwordHasher = passwordHasher;
        _configuration = configuration;

    }

    public async Task<OperationResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            return OperationResult<AuthResponse>.Fail("Invalid credentials", HttpStatusCode.Unauthorized);

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

        if (result == PasswordVerificationResult.Failed)
            return OperationResult<AuthResponse>.Fail("Invalid credentials", HttpStatusCode.Unauthorized);

        var token = GenerateJwtToken(user);

        return OperationResult<AuthResponse>.Success(new AuthResponse
        {
            Email = user.Email,
            DisplayName = user.DisplayName,
            Token = token
        });
    }

    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            throw new InvalidOperationException("JWT key must be at least 256 bits (32 characters)");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<OperationResult<User>> RegisterUserAsync(RegisterUserRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return OperationResult<User>.Fail("Passwords do not match");

        var existing = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existing != null)
            return OperationResult<User>.Fail("Email already registered");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            DisplayName = request.DisplayName,
            Role = UserRole.User,
            AuthProvider = "local"
        };

        var hashedPassword = _passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = hashedPassword;

        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();

        return OperationResult<User>.Success(user);
    }

    public async Task<OperationResult<User>> RegisterAdminAsync(RegisterAdminRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return OperationResult<User>.Fail("Passwords do not match");

        var existing = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existing != null)
            return OperationResult<User>.Fail("Email already registered");

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            DisplayName = request.DisplayName,
            Role = UserRole.Admin,
            AuthProvider = "local"
        };

        adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, request.Password);

        _appDbContext.Users.Add(adminUser);
        await _appDbContext.SaveChangesAsync();

        return OperationResult<User>.Success(adminUser);
    }

}
