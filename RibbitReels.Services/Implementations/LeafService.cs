
using System.Net;
using Microsoft.EntityFrameworkCore;
using RibbitReels.Data;
using RibbitReels.Data.Models;
using RibbitReels.Services.Interfaces;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Implementations;

public class LeafService : ILeafService
{
    private readonly AppDbContext _appDbContext;

    public LeafService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<OperationResult<Leaf>> CreateLeafAsync(Guid branchId, Leaf leaf)
    {
        try
        {
            // input validation
            if (string.IsNullOrWhiteSpace(leaf.Title) || string.IsNullOrWhiteSpace(leaf.VideoUrl))
            {
                return OperationResult<Leaf>.Fail("Leaf must have a title and video URL.", HttpStatusCode.BadRequest);
            }

            // every leaf have to reference a branch
            var branch = await _appDbContext.Branches.FindAsync(branchId);
            if (branch == null)
            {
                return OperationResult<Leaf>.Fail("Branch not found.", HttpStatusCode.NotFound);
            }

            // assign the branchId to the leaf
            leaf.BranchId = branchId;

            // initializing id
            if (leaf.Id == Guid.Empty)
            {
                leaf.Id = Guid.NewGuid();
            }

            // save
            _appDbContext.Leaves.Add(leaf);
            await _appDbContext.SaveChangesAsync();

            return OperationResult<Leaf>.Success(leaf, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return OperationResult<Leaf>.Fail(ex, "Failed to create leaf.");
        }
    }

    public async Task<OperationResult<Leaf>> GetLeafByIdAsync(Guid id)
    {
        try
        {
            var leaf = await _appDbContext.Leaves
                .Include(l => l.Branch)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaf == null)
            {
                return OperationResult<Leaf>.Fail("Leaf not found.", HttpStatusCode.NotFound);
            }

            return OperationResult<Leaf>.Success(leaf, HttpStatusCode.OK);
        }
        catch (Exception)
        {
            return OperationResult<Leaf>.Fail("An error occurred while retrieving the leaf.", HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<List<Leaf>>> GetLeavesByBranchIdAsync(Guid branchId)
    {
        try
        {
            var leaves = await _appDbContext.Leaves
                .Where(l => l.BranchId == branchId)
                .OrderBy(l => l.Order)
                .ToListAsync();

            return OperationResult<List<Leaf>>.Success(leaves);
        }
        catch (Exception)
        {
            return OperationResult<List<Leaf>>.Fail("Failed to retrieve leaves for the specified branch.", HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<Leaf>> UpdateLeafAsync(Guid id, Leaf updatedLeaf)
    {
        try
        {
            var existingLeaf = await _appDbContext.Leaves.FindAsync(id);
            if (existingLeaf == null)
            {
                return OperationResult<Leaf>.Fail("Leaf not found", HttpStatusCode.NotFound);
            }

            existingLeaf.Title = updatedLeaf.Title;
            existingLeaf.VideoUrl = updatedLeaf.VideoUrl;
            existingLeaf.Order = updatedLeaf.Order;

            // save changes
            await _appDbContext.SaveChangesAsync();

            return OperationResult<Leaf>.Success(existingLeaf);
        }
        catch (Exception)
        {
            return OperationResult<Leaf>.Fail("An error occurred while updating the leaf.", HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<bool>> DeleteLeafAsync(Guid id)
    {
        var leaf = await _appDbContext.Leaves.FindAsync(id);

        if (leaf == null)
        {
            return OperationResult<bool>.Fail("Leaf not found", HttpStatusCode.NotFound);
        }

        _appDbContext.Leaves.Remove(leaf);

        try
        {
            await _appDbContext.SaveChangesAsync();
            return OperationResult<bool>.Success(true, HttpStatusCode.OK);
        }
        catch (Exception)
        {
            return OperationResult<bool>.Fail("An error occurred while deleting the leaf", HttpStatusCode.BadRequest);
        }
    }

}