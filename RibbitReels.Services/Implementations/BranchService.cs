using System.Net;
using Microsoft.EntityFrameworkCore;
using RibbitReels.Data;
using RibbitReels.Data.Models;
using RibbitReels.Services.Interfaces;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Implementations;

public class BranchService : IBranchService
{
    private readonly AppDbContext _appDbContext;

    public BranchService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<OperationResult<Branch>> CreateBranchAsync(Branch branch)
    {
        try
        {
            // input validation - will move this though
            if (string.IsNullOrWhiteSpace(branch.Title) || string.IsNullOrWhiteSpace(branch.Description))
            {
                return OperationResult<Branch>.Fail("Branch title and description are required.", HttpStatusCode.BadRequest);
            }

            // initialize id
            if (branch.Id == Guid.Empty)
            {
                branch.Id = Guid.NewGuid();
            }

            // add to db
            _appDbContext.Branches.Add(branch);
            await _appDbContext.SaveChangesAsync();

            return OperationResult<Branch>.Success(branch, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return OperationResult<Branch>.Fail(ex, "Failed to create branch.");
        }
    }
    
    public async Task<OperationResult<Branch>> UpdateBranchAsync(Guid id, Branch updatedBranch)
    {
        try
        {
            var existingBranch = await _appDbContext.Branches.FindAsync(id);

            if (existingBranch == null)
            {
                return OperationResult<Branch>.Fail("Branch not found.", HttpStatusCode.NotFound);
            }

            // input changes validation here 
            if (string.IsNullOrWhiteSpace(updatedBranch.Title) || string.IsNullOrWhiteSpace(updatedBranch.Description))
            {
                return OperationResult<Branch>.Fail("Title and description cannot be empty.", HttpStatusCode.BadRequest);
            }

            // then we update the fields
            existingBranch.Title = updatedBranch.Title;
            existingBranch.Description = updatedBranch.Description;

            _appDbContext.Branches.Update(existingBranch);
            await _appDbContext.SaveChangesAsync();

            return OperationResult<Branch>.Success(existingBranch, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return OperationResult<Branch>.Fail(ex, "Failed to update branch.");
        }
    }


    public async Task<OperationResult<bool>> DeleteBranchAsync(Guid id)
    {
        try
        {
            var branch = await _appDbContext.Branches
                .Include(b => b.Leaves)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                return OperationResult<bool>.Fail("Branch not found.", HttpStatusCode.NotFound);
            }

            // removing associated leaves
            _appDbContext.Leaves.RemoveRange(branch.Leaves);

            // removing the branch now
            _appDbContext.Branches.Remove(branch);

            await _appDbContext.SaveChangesAsync();

            return OperationResult<bool>.Success(true, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return OperationResult<bool>.Fail(ex, "Failed to delete branch.");
        }
    }


    public async Task<OperationResult<List<Branch>>> GetAllBranchesAsync()
    {
        try
        {
            var branches = await _appDbContext.Branches
                .AsNoTracking()
                .Include(b => b.Leaves) // this will include leaves for that branch - will remove when branches become many
                .ToListAsync();

            return OperationResult<List<Branch>>.Success(branches, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return OperationResult<List<Branch>>.Fail(ex, "Failed to retrieve branches.");
        }
    }


    public async Task<OperationResult<Branch>> GetBranchByIdAsync(Guid id)
    {
        try
        {
            var branch = await _appDbContext.Branches
                .Include(b => b.Leaves)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (branch == null)
            {
                return OperationResult<Branch>.Fail($"Branch with ID {id} not found.", HttpStatusCode.NotFound);
            }

            return OperationResult<Branch>.Success(branch, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return OperationResult<Branch>.Fail(ex, "Failed to retrieve branch.");
        }
    }

    public Task<OperationResult<List<Branch>>> GetCompletedBranchesAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<List<Branch>>> GetIncompleteBranchesAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<List<Branch>>> GetRecommendedBranchesAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}