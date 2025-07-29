
using RibbitReels.Data.DTOs;
using RibbitReels.Data.Models;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Interfaces;

public interface IUserService
{
    Task<OperationResult<User>> RegisterUserAsync(RegisterUserRequest request);
    Task<OperationResult<User>> RegisterAdminAsync(RegisterAdminRequest request);
    Task<OperationResult<AuthResponse>> LoginAsync(LoginRequest request);

    Task<OperationResult<AuthResponse>> LoginWithGoogleAsync(GoogleLoginRequest request);

}