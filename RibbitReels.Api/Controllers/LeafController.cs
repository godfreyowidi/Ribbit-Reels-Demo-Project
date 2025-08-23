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

      // POST: api/leafs/{branchId}/manual
    [HttpPost("leafs/{branchId}/manual")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateManualLeaf(Guid branchId, [FromForm] CreateManualLeafRequest request)
    {
        if (request.VideoFile == null || request.VideoFile.Length == 0)
            return BadRequest(new { error = "Video file is required." });

        var leaf = new Leaf
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            Title = request.Title,
            Description = request.Description,
            Order = request.Order,
            Source = "Manual"
        };

        var result = await _leafService.CreateManualLeafAsync(branchId, request);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        var createdLeaf = result.Value;

        return CreatedAtAction(nameof(GetLeafById), new { id = createdLeaf.Id }, new LeafResponse
        {
            Id = createdLeaf.Id,
            BranchId = createdLeaf.BranchId,
            Title = createdLeaf.Title,
            Description = createdLeaf.Description ?? string.Empty,
            VideoUrl = Url.Action(nameof(GetVideo), new { id = createdLeaf.Id }),
            Order = createdLeaf.Order,
            Source = createdLeaf.Source,
            Status = createdLeaf.Status,
            CreatedAt = createdLeaf.CreatedAt
        });
    }

    [HttpPost("leafs/{branchId}/youtube")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddYouTubeLeaf(Guid branchId, [FromBody] AddYouTubeLeafRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.VideoId))
            return BadRequest(new { error = "YT VideoId is required" });

        var videoResult = await _youTubeRepository.GetVideoDetailsAsync(request.VideoId);
        if (!videoResult.IsSuccessful)
            return StatusCode(videoResult.StatusCode, new { error = videoResult.FailureMessage });

        var ytVideo = videoResult.Value;

        var result = await _leafService.CreateYouTubeLeafAsync(branchId, ytVideo);       
        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        var created = result.Value;

        return CreatedAtAction(nameof(GetLeafById), new { id = created.Id }, new LeafResponse
        {
            Id = created.Id,
            BranchId = created.BranchId,
            Title = created.Title,
            Description = created.Description ?? string.Empty,
            VideoUrl = null,
            ThumbnailUrl = created.ThumbnailUrl,
            Order = created.Order,
            Source = created.Source,
            YouTubeVideoId = created.YouTubeVideoId,
            Status = created.Status,
            CreatedAt = created.CreatedAt
        });
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
            BranchId = leaf.BranchId,
            Description = leaf.Description ?? string.Empty,
            Title = leaf.Title,
            VideoUrl = leaf.Source == "Manual" ? Url.Action(nameof(GetVideo), new { id = leaf.Id }) : null,
            ThumbnailUrl = leaf.ThumbnailUrl,
            Order = leaf.Order,
            Source = leaf.Source,
            YouTubeVideoId = leaf.YouTubeVideoId,
            Status = leaf.Status,
            CreatedAt = leaf.CreatedAt
        });
    }

    // GET: api/leaf/{id}/video
    [HttpGet("{id}/video")]
    public async Task<IActionResult> GetVideo(Guid id)
    {
        var result = await _leafService.GetLeafByIdAsync(id);
        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        var leaf = result.Value;

        if (leaf.VideoData == null)
            return NotFound(new { error = result.FailureMessage });

        return File(leaf.VideoData, leaf.VideoContentType ?? "application/octet-stream", leaf.VideoFileName);
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
