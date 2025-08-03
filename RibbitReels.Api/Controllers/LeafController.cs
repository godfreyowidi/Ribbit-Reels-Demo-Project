using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RibbitReels.Api.DTOs;
using RibbitReels.Data.DTOs;
using RibbitReels.Data.Models;
using RibbitReels.Services.Interfaces;

namespace RibbitReels.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeafController : ControllerBase
{
    private readonly ILeafService _leafService;

    public LeafController(ILeafService leafService)
    {
        _leafService = leafService;
    }

    // POST : api/leaf/{branchId}
    [HttpPost("{branchId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateLeaf(Guid branchId, [FromBody] CreateLeafRequest request)
    {
        var leaf = new Leaf
        {
            Id = Guid.NewGuid(),
            BranchId = request.BranchId,
            Title = request.Title,
            VideoUrl = request.VideoUrl,
            Order = request.Order
        };

        var result = await _leafService.CreateLeafAsync(branchId, leaf);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        return StatusCode(result.StatusCode, new LeafResponse
        {
            Id = result.Value.Id,
            Title = result.Value.Title,
            VideoUrl = result.Value.VideoUrl,
            Order = result.Value.Order,
            BranchId = result.Value.BranchId
        });
    }

    // GET: api/{id}
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

    // GET: api/branch?/{branchId}
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
