using System.Net;
using Microsoft.EntityFrameworkCore;
using RibbitReels.Data;
using RibbitReels.Data.DTOs;
using RibbitReels.Data.Models;
using RibbitReels.Services.Interfaces;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Implementations;

public class LearningProgressService : ILearningProgressService
{
    private readonly AppDbContext _appDbContext;

    public LearningProgressService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<OperationResult<LearningProgressResponse>> GetProgressAsync(Guid userId, Guid branchId)
    {
        var progress = await _appDbContext.UserProgress
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && p.BranchId == branchId);

        if (progress == null)
        {
            return OperationResult<LearningProgressResponse>.Fail("No progress found", HttpStatusCode.NotFound);
        }

        var totalLeaves = await _appDbContext.Leaves
            .Where(l => l.BranchId == branchId)
            .CountAsync();

        double percentageCompleted = 0;

        if (totalLeaves > 0 && progress.CompletedLeafIds != null)
        {
            percentageCompleted = progress.CompletedLeafIds.Count / (double)totalLeaves * 100;
        }

        return OperationResult<LearningProgressResponse>.Success(new LearningProgressResponse
        {
            UserId = userId,
            BranchId = branchId,
            CompletedLeafIds = progress.CompletedLeafIds ?? new List<Guid>(),
            CompletedAt = progress.CompletedAt,
            PercentageCompleted = Math.Round(percentageCompleted, 2)
        });
    }


    public async Task<OperationResult<LearningProgressResponse>> UpdateProgressAsync(UpdateProgressRequest request)
    {
        var progress = await _appDbContext.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == request.UserId && p.BranchId == request.BranchId);

        if (progress == null)
        {
            progress = new LearningProgress
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                BranchId = request.BranchId,
                CompletedLeafIds = request.CompletedLeafIds?.Distinct().ToList() ?? new List<Guid>(),
                CompletedAt = request.CompletedAt
            };

            _appDbContext.UserProgress.Add(progress);
        }
        else
        {
            var updatedIds = progress.CompletedLeafIds.Union(request.CompletedLeafIds ?? new List<Guid>()).Distinct().ToList();
            progress.CompletedLeafIds = updatedIds;
            progress.CompletedAt = request.CompletedAt;
            _appDbContext.UserProgress.Update(progress);
        }

        await _appDbContext.SaveChangesAsync();

        return OperationResult<LearningProgressResponse>.Success(new LearningProgressResponse
        {
            UserId = request.UserId,
            BranchId = request.BranchId,
            CompletedLeafIds = progress.CompletedLeafIds,
            CompletedAt = progress.CompletedAt
        });
    }

    public async Task<OperationResult<List<CompletedBranchResponse>>> GetCompletedBranchesAsync(Guid userId)
    {
        var completed = await _appDbContext.UserProgress
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.CompletedAt != null)
            .Select(p => new CompletedBranchResponse
            {
                BranchId = p.BranchId,
                CompletedAt = p.CompletedAt
            })
            .ToListAsync();

        return OperationResult<List<CompletedBranchResponse>>.Success(completed);
    }
}
