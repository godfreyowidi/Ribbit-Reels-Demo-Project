using RibbitReels.Data.DTOs;
using RibbitReels.Data.Models;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Interfaces;

public interface IUserService
{
    // Auth
    Task<OperationResult<User>> RegisterUserAsync(RegisterUserRequest request);
    Task<OperationResult<User>> RegisterAdminAsync(RegisterAdminRequest request);
    Task<OperationResult<AuthResponse>> LoginAsync(LoginRequest request);
    Task<OperationResult<AuthResponse>> LoginWithGoogleAsync(GoogleLoginRequest request);

    // CRUD
    Task<OperationResult<IEnumerable<User>>> GetAllUsersAsync();
    Task<OperationResult<User>> GetUserByIdAsync(Guid userId);
    Task<OperationResult<User>> UpdateUserAsync(Guid userId, UpdateUserRequest request);
    Task<OperationResult<bool>> DeleteUserAsync(Guid userId);
}
