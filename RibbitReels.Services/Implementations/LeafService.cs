using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

    public async Task<OperationResult<Leaf>> CreateManualLeafAsync(Guid branchId, Leaf leaf, IFormFile? videoFile = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(leaf.Title))
                return OperationResult<Leaf>.Fail("Leaf must have a title.", HttpStatusCode.BadRequest);

            var branch = await _appDbContext.Branches.FindAsync(branchId);
            if (branch == null)
                return OperationResult<Leaf>.Fail("Branch not found.", HttpStatusCode.NotFound);

            leaf.BranchId = branchId;
            if (leaf.Id == Guid.Empty)
                leaf.Id = Guid.NewGuid();
            leaf.Source = "Manual";

            if (videoFile != null)
            {
                var uploadResult = await _blobRepository.UploadVideoAsync(videoFile, leaf.Id.ToString());
                if (!uploadResult.IsSuccessful)
                    return OperationResult<Leaf>.Fail("failed to upload file", HttpStatusCode.BadRequest);

                // we store only the relative path in DB
                leaf.VideoUrl = uploadResult.Value;
            }

            _appDbContext.Leafs.Add(leaf);
            await _appDbContext.SaveChangesAsync();

            return OperationResult<Leaf>.Success(leaf, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return OperationResult<Leaf>.Fail(ex, "Failed to create leaf.");
        }
    }

    public async Task<OperationResult<Leaf>> CreateYouTubeLeafAsync(Guid branchId, YouTubeVideo video)
    {
        try
        {
            var branch = await _appDbContext.Branches.FindAsync(branchId);
            if (branch == null)
                return OperationResult<Leaf>.Fail("Branch not found.", HttpStatusCode.NotFound);

            var videoUrl = $"https://www.youtube.com/watch?v={video.VideoId}";

            // we check if this YT video already exists in/for this branch
            var existingLeaf = await _appDbContext.Leafs
                .FirstOrDefaultAsync(l => l.BranchId == branchId && l.VideoUrl == videoUrl);

            if (existingLeaf != null)
            {
                return OperationResult<Leaf>.Fail(
                    "This YouTube video has already been added to the branch.",
                    HttpStatusCode.Conflict
                );
            }

            var leaf = new Leaf
            {
                Id = Guid.NewGuid(),
                BranchId = branchId,
                Title = video.Title,
                Description = video.Description,
                ThumbnailUrl = video.ThumbnailUrl,
                VideoUrl = videoUrl,
                Source = "YouTube",
                Order = await _appDbContext.Leafs
                    .Where(l => l.BranchId == branchId)
                    .CountAsync() + 1 // sequential ordering per branch
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

            if (!string.IsNullOrWhiteSpace(leaf.VideoUrl))
            {
                var sasResult = await _blobRepository.GetVideoUrlAsync(leaf.VideoUrl);
                if (sasResult.IsSuccessful)
                    leaf.VideoUrl = sasResult.Value;
            }

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
        catch (Exception)
        {
            return OperationResult<List<Leaf>>.Fail("Failed to retrieve leaves for the specified branch.", HttpStatusCode.BadRequest);
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