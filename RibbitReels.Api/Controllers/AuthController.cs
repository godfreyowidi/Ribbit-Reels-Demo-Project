using Microsoft.AspNetCore.Mvc;
using RibbitReels.Data.DTOs;
using RibbitReels.Services.Interfaces;
using RibbitReels.Data.Models;

namespace RibbitReels.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _userService.RegisterUserAsync(request);

        if (!result.IsSuccessful)
            return BadRequest(new { error = result.FailureMessage });

        return Ok(new { userId = result.Value.Id, email = result.Value.Email });
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
    {
        var result = await _userService.RegisterAdminAsync(request);

        if (!result.IsSuccessful)
            return BadRequest(new { error = result.FailureMessage });

        return Ok(new { adminId = result.Value.Id, email = result.Value.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);

        if (!result.IsSuccessful)
            return StatusCode(result.StatusCode, new { error = result.FailureMessage });

        return Ok(result.Value);
    }
}
