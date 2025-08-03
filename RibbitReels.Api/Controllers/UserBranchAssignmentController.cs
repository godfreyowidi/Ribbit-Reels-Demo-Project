using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RibbitReels.Data.DTOs;
using RibbitReels.Services.Interfaces;

namespace RibbitReels.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserBranchAssignmentController : ControllerBase
{
    private readonly IUserBranchAssignmentService _assignmentService;

    public UserBranchAssignmentController(IUserBranchAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    // POST: api/UserBranchAssignment/assign
    [HttpPost("assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignBranch([FromBody] AssignBranchRequest request)
    {
        var result = await _assignmentService.AssignBranchAsync(request);
        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, result.FailureMessage);

        return Ok(result.Value);
    }

    // GET: api/UserBranchAssignment/user/{userId}
    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetAssignmentsByUser(Guid userId)
    {
        var result = await _assignmentService.GetAssignmentsByUserAsync(userId);
        if (!result.IsSuccessful)
            return StatusCode((int)result.StatusCode, result.FailureMessage);

        return Ok(result.Value);
    }

    // GET: api/UserBranchAssignment/manager/{managerId}
    [HttpGet("manager/{managerId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAssignmentsByManager(Guid managerId)
    {
        var result = await _assignmentService.GetAssignmentsByManagerAsync(managerId);
        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, result.FailureMessage);

        return Ok(result.Value);
    }
}
