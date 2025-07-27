

using RibbitReels.Data.Models;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Interfaces;

public interface IBranchService
{
    Task<OperationResult<Branch>> CreateBranchAsync(Branch branch);
    Task<OperationResult<Branch>> GetBranchByIdAsync(Guid id);
    Task<OperationResult<List<Branch>>> GetAllBranchesAsync();
    Task<OperationResult<Branch>> UpdateBranchAsync(Guid id, Branch updatedBranch);
    Task<OperationResult<bool>> DeleteBranchAsync(Guid id);

    Task<OperationResult<List<Branch>>> GetCompletedBranchesAsync(Guid userId);
    Task<OperationResult<List<Branch>>> GetIncompleteBranchesAsync(Guid userId);
    Task<OperationResult<List<Branch>>> GetRecommendedBranchesAsync(Guid userId);
}


