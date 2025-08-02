using RibbitReels.Data.DTOs;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Interfaces;

public interface IUserBranchAssignmentService
{
    Task<OperationResult<UserBranchAssignmentResponse>> AssignBranchAsync(AssignBranchRequest request);
    Task<OperationResult<List<UserBranchAssignmentResponse>>> GetAssignmentsByUserAsync(Guid userId);
    Task<OperationResult<bool>> UnassignBranchAsync(Guid userId, Guid branchId);
    Task<OperationResult<List<UserBranchAssignmentResponse>>> GetAssignmentsByManagerAsync(Guid managerId);
}
