using System.Net;
using Microsoft.EntityFrameworkCore;
using RibbitReels.Data;
using RibbitReels.Data.DTOs;
using RibbitReels.Data.Models;
using RibbitReels.Services.Interfaces;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Implementations;

public class UserBranchAssignmentService : IUserBranchAssignmentService
{
    private readonly AppDbContext _appDbContext;

    public UserBranchAssignmentService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<OperationResult<UserBranchAssignmentResponse>> AssignBranchAsync(AssignBranchRequest request)
    {
        // Check if user exists
        var user = await _appDbContext.Users.FindAsync(request.UserId);
        if (user == null)
            return OperationResult<UserBranchAssignmentResponse>.Fail("User not found", HttpStatusCode.NotFound);

        // Check if branch exists
        var branch = await _appDbContext.Branches.FindAsync(request.BranchId);
        if (branch == null)
            return OperationResult<UserBranchAssignmentResponse>.Fail("Branch not found", HttpStatusCode.NotFound);

        // Check if manager exists
        var manager = await _appDbContext.Users.FindAsync(request.AssignedByManagerId);
        if (manager == null || manager.Role != UserRole.Admin)
            return OperationResult<UserBranchAssignmentResponse>.Fail("Invalid assigning manager", HttpStatusCode.Forbidden);

        // Prevent duplicate assignment
        var existingAssignment = await _appDbContext.AssignedBranches
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.BranchId == request.BranchId);

        if (existingAssignment != null)
            return OperationResult<UserBranchAssignmentResponse>.Fail("Branch already assigned to user", HttpStatusCode.Conflict);

        // Create new assignment
        var assignment = new UserBranchAssignment
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            BranchId = request.BranchId,
            AssignedByManagerId = request.AssignedByManagerId
        };

        _appDbContext.AssignedBranches.Add(assignment);
        await _appDbContext.SaveChangesAsync();

        return OperationResult<UserBranchAssignmentResponse>.Success(new UserBranchAssignmentResponse
        {
            UserId = assignment.UserId,
            BranchId = assignment.BranchId,
            AssignedByManagerId = assignment.AssignedByManagerId
        });
    }

    public async Task<OperationResult<List<UserBranchAssignmentResponse>>> GetAssignmentsByManagerAsync(Guid managerId)
    {
        var assignments = await _appDbContext.AssignedBranches
            .Where(x => x.AssignedByManagerId == managerId)
            .Select(a => new UserBranchAssignmentResponse
            {
                UserId = a.UserId,
                BranchId = a.BranchId,
                AssignedByManagerId = a.AssignedByManagerId
            })
            .ToListAsync();

        return OperationResult<List<UserBranchAssignmentResponse>>.Success(assignments);
    }

    public async Task<OperationResult<List<UserBranchAssignmentResponse>>> GetAssignmentsByUserAsync(Guid userId)
    {
        var assignments = await _appDbContext.AssignedBranches
            .Where(x => x.UserId == userId)
            .Select(a => new UserBranchAssignmentResponse
            {
                UserId = a.UserId,
                BranchId = a.BranchId,
                AssignedByManagerId = a.AssignedByManagerId
            })
            .ToListAsync();

        return OperationResult<List<UserBranchAssignmentResponse>>.Success(assignments);
    }

}