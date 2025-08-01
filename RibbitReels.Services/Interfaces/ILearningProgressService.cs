using RibbitReels.Data.DTOs;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Interfaces;

public interface ILearningProgressService
{
    Task<OperationResult<LearningProgressResponse>> GetProgressAsync(Guid userId, Guid branchId);
    Task<OperationResult<LearningProgressResponse>> UpdateProgressAsync(UpdateProgressRequest request);
    Task<OperationResult<List<CompletedBranchResponse>>> GetCompletedBranchesAsync(Guid userId);

}
