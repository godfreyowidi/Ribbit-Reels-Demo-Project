using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RibbitReels.Api.DTOs;
using RibbitReels.Data.DTOs;
using RibbitReels.Data.Models;
using RibbitReels.Services.Implementations;
using RibbitReels.Services.Interfaces;

namespace RibbitReels.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeafController : ControllerBase
{
    private readonly ILeafService _leafService;
     private readonly YouTubeRepository _youTubeRepository;

    public LeafController(ILeafService leafService, YouTubeRepository youTubeRepository)
    {
        _leafService = leafService;
        _youTubeRepository = youTubeRepository;
    }

    // POST: api/branches/{branchId}/leaves
    [HttpPost("/api/branches/{branchId}/leaves")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLeaf(Guid branchId, [FromForm] CreateLeafRequest request)
    {
        var leaf = new Leaf
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            Title = request.Title,
            Order = request.Order
        };

        var result = await _leafService.CreateManualLeafAsync(branchId, leaf, request.VideoFile);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        return CreatedAtAction(nameof(GetLeafById), new { id = result.Value.Id }, new LeafResponse
        {
            Id = result.Value.Id,
            Title = result.Value.Title,
            VideoUrl = result.Value.VideoUrl,
            Order = result.Value.Order,
            BranchId = result.Value.BranchId
        });
    }

    [HttpPost("/api/branches/{branchId}/leaves/youtube")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddYouTubeLeaf(Guid branchId, [FromBody] AddYouTubeLeafRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.VideoId))
            return BadRequest(new { error = "YT VideoId is required" });

        var videoResult = await _youTubeRepository.GetVideoDetailsAsync(request.VideoId);
        if (!videoResult.IsSuccessful)
            return StatusCode(videoResult.StatusCode, new { error = videoResult.FailureMessage });

        var result = await _leafService.CreateYouTubeLeafAsync(branchId, videoResult.Value);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        var leaf = result.Value;

        return CreatedAtAction(nameof(GetLeafById), new { id = leaf.Id }, new LeafResponse
        {
            Id = leaf.Id,
            BranchId = leaf.BranchId,
            Title = leaf.Title,
            Text = leaf.Description ?? string.Empty,
            VideoUrl = leaf.VideoUrl,
            ThumbnailUrl = leaf.ThumbnailUrl,
            Order = leaf.Order,
            Source = leaf.Source,
            Status = leaf.Status,
            CreatedAt = leaf.CreatedAt
        });
    }

    [HttpGet("/api/youtube/search")]
    public async Task<IActionResult> SearchForYoutubeVideo([FromQuery] string q, [FromQuery] int maxResults = 5)
    {
        var result = await _youTubeRepository.SearchVideosAsync(q, maxResults);
        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, result.FailureMessage);

        return Ok(result.Value);
    }


    // GET: api/leaf/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLeafById(Guid id)
    {
        var result = await _leafService.GetLeafByIdAsync(id);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        var leaf = result.Value;

        return Ok(new LeafResponse
        {
            Id = leaf.Id,
            Title = leaf.Title,
            VideoUrl = leaf.VideoUrl,
            Order = leaf.Order,
            BranchId = leaf.BranchId
        });
    }


    // GET: api/leaf
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetLeafs()
    {
        var result = await _leafService.GetLeafsAsync();
        return result.IsSuccessful ? Ok(new { data = result.Value }) : BadRequest(result.FailureMessage);
    }

    // GET: api/branch/{branchId}
    [HttpGet("branch/{branchId:guid}")]
    public async Task<IActionResult> GetLeafsByBranchId(Guid branchId)
    {
        var result = await _leafService.GetLeafsByBranchIdAsync(branchId);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        var responses = result.Value.Select(leaf => new LeafResponse
        {
            Id = leaf.Id,
            BranchId = leaf.BranchId,
            Title = leaf.Title,
            VideoUrl = leaf.VideoUrl,
            Order = leaf.Order
        }).ToList();

        return Ok(responses);
    }

    // PUT: /api/leaf/{id}
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLeaf(Guid id, [FromBody] UpdateLeafRequest request)
    {
        var updatedLeaf = new Leaf
        {
            Title = request.Title,
            VideoUrl = request.VideoUrl,
            Order = request.Order
        };

        var result = await _leafService.UpdateLeafAsync(id, updatedLeaf);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        var leaf = result.Value;
        return Ok(new LeafResponse
        {
            Id = leaf.Id,
            BranchId = leaf.BranchId,
            Title = leaf.Title,
            VideoUrl = leaf.VideoUrl,
            Order = leaf.Order
        });
    }

    // DELETE: /api/leaf/{id}
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLeaf(Guid id)
    {
        var result = await _leafService.DeleteLeafAsync(id);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        return NoContent();
    }

}
