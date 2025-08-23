using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RibbitReels.Api.DTOs;
using RibbitReels.Data;
using RibbitReels.Data.DTOs;
using RibbitReels.Data.Models;
using RibbitReels.Services.Interfaces;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Implementations;

public class LeafService : ILeafService
{
    private readonly AppDbContext _appDbContext;
    private readonly AzureBlobRepository _blobRepository;

    public LeafService(AppDbContext appDbContext, AzureBlobRepository blobRepository)
    {
        _appDbContext = appDbContext;
        _blobRepository = blobRepository;
    }

    public async Task<OperationResult<Leaf>> CreateManualLeafAsync(Guid branchId, CreateManualLeafRequest request)
    {
        try
        {
            var branch = await _appDbContext.Branches.FindAsync(branchId);
            if (branch == null)
                return OperationResult<Leaf>.Fail("Branch not found.", HttpStatusCode.NotFound);

            var nextOrder = request.Order > 0
                ? request.Order
                : await _appDbContext.Leafs.CountAsync(l => l.BranchId == branchId) + 1;

            var leaf = new Leaf
            {
                Id = Guid.NewGuid(),
                BranchId = branchId,
                Title = string.IsNullOrWhiteSpace(request.Title) ? "Untitled Video" : request.Title[..Math.Min(255, request.Title.Length)],
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description[..Math.Min(2000, request.Description.Length)],
                Source = "Manual",
                Status = "Published",
                Order = nextOrder,
                CreatedAt = DateTime.UtcNow
            };

            if (request.VideoFile != null)
            {
                using var ms = new MemoryStream();
                await request.VideoFile.CopyToAsync(ms);

                leaf.VideoData = ms.ToArray();
                leaf.VideoFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.VideoFile.FileName)}";
                leaf.VideoContentType = request.VideoFile.ContentType;
            }

            _appDbContext.Leafs.Add(leaf);
            await _appDbContext.SaveChangesAsync();

            return OperationResult<Leaf>.Success(leaf, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return OperationResult<Leaf>.Fail(ex, "Failed to create manual leaf.");
        }
    }

    public async Task<OperationResult<Leaf>> CreateYouTubeLeafAsync(Guid branchId, YouTubeVideo video)
    {
        try
        {
            var branch = await _appDbContext.Branches.FindAsync(branchId);
            if (branch == null)
                return OperationResult<Leaf>.Fail("Branch not found.", HttpStatusCode.NotFound);

            // check for duplicates
            var exists = await _appDbContext.Leafs
                .AnyAsync(l => l.BranchId == branchId && l.YouTubeVideoId == video.VideoId);
            if (exists)
                return OperationResult<Leaf>.Fail("This YouTube video already exists in this branch.", HttpStatusCode.Conflict);

            var nextOrder = await _appDbContext.Leafs.CountAsync(l => l.BranchId == branchId) + 1;

            var leaf = new Leaf
            {
                Id = Guid.NewGuid(),
                BranchId = branchId,
                Title = string.IsNullOrWhiteSpace(video.Title) ? "Untitled Video" : video.Title[..Math.Min(255, video.Title.Length)],
                Description = string.IsNullOrWhiteSpace(video.Description) ? null : video.Description[..Math.Min(2000, video.Description.Length)],
                ThumbnailUrl = video.ThumbnailUrl,
                YouTubeVideoId = video.VideoId,
                Source = "YouTube",
                Status = "Published",
                Order = nextOrder,
                CreatedAt = DateTime.UtcNow
            };

            _appDbContext.Leafs.Add(leaf);
            await _appDbContext.SaveChangesAsync();

            return OperationResult<Leaf>.Success(leaf, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return OperationResult<Leaf>.Fail(ex, "Failed to create YouTube leaf.");
        }
    }

    public async Task<OperationResult<Leaf>> GetLeafByIdAsync(Guid id)
    {
        try
        {
            var leaf = await _appDbContext.Leafs
                .Include(l => l.Branch)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaf == null)
                return OperationResult<Leaf>.Fail("Leaf not found.", HttpStatusCode.NotFound);

            return OperationResult<Leaf>.Success(leaf, HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            return OperationResult<Leaf>.Fail($"An error occurred while retrieving the leaf: {ex.Message}", HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<List<Leaf>>> GetLeafsAsync()
    {
        try
        {
            var leaves = await _appDbContext.Leafs
                .Include(l => l.Branch)
                .OrderBy(l => l.Order)
                .ToListAsync();

            return OperationResult<List<Leaf>>.Success(leaves);
        }
        catch (Exception)
        {
            return OperationResult<List<Leaf>>.Fail("Failed to retrieve leaves.", HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<List<Leaf>>> GetLeafsByBranchIdAsync(Guid branchId)
    {
        try
        {
            var leaves = await _appDbContext.Leafs
                .Where(l => l.BranchId == branchId)
                .OrderBy(l => l.Order)
                .ToListAsync();

            return OperationResult<List<Leaf>>.Success(leaves);
        }
        catch (Exception ex)
        {
            return OperationResult<List<Leaf>>.Fail($"Failed to retrieve leaves for the specified branch. {ex.Message}", HttpStatusCode.BadRequest);
        }
    }

    public async Task<OperationResult<Leaf>> UpdateLeafAsync(Guid id, Leaf updatedLeaf)
    {
        try
        {
            var existingLeaf = await _appDbContext.Leafs.FindAsync(id);
            if (existingLeaf == null)
                return OperationResult<Leaf>.Fail("Leaf not found", HttpStatusCode.NotFound);

            existingLeaf.Title = updatedLeaf.Title;
            existingLeaf.Description = updatedLeaf.Description ?? existingLeaf.Description;
            existingLeaf.Order = updatedLeaf.Order != 0 ? updatedLeaf.Order : existingLeaf.Order;
            existingLeaf.Status = updatedLeaf.Status ?? existingLeaf.Status;

            if (updatedLeaf.VideoData != null && updatedLeaf.VideoData.Length > 0)
            {
                existingLeaf.VideoData = updatedLeaf.VideoData;
                existingLeaf.VideoFileName = updatedLeaf.VideoFileName ?? existingLeaf.VideoFileName;
                existingLeaf.VideoContentType = updatedLeaf.VideoContentType ?? existingLeaf.VideoContentType;
            }

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
        var leaf = await _appDbContext.Leafs.FindAsync(id);

        if (leaf == null)
            return OperationResult<bool>.Fail("Leaf not found", HttpStatusCode.NotFound);

        _appDbContext.Leafs.Remove(leaf);

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