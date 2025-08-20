using Microsoft.AspNetCore.Mvc;
using RibbitReels.Data.DTOs;
using RibbitReels.Services.Implementations;

namespace RibbitReels.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchAllController : ControllerBase
{
    private readonly YouTubeRepository _youtubeRepo;
    private readonly UdemyRepository _udemyRepo;

    public SearchAllController(YouTubeRepository youtubeRepo, UdemyRepository udemyRepo)
    {
        _youtubeRepo = youtubeRepo;
        _udemyRepo = udemyRepo;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAll([FromQuery] string q, [FromQuery] int maxResults = 5)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Query cannot be empty." });

        var ytTask = _youtubeRepo.SearchVideosAsync(q, maxResults);
        var udemyTask = _udemyRepo.SearchCoursesAsync(q, 1, maxResults);

        await Task.WhenAll(ytTask, udemyTask);

        var ytResult = ytTask.Result;
        var udemyResult = udemyTask.Result;

        var response = new SearchAllResponse
        {
            YouTube = ytResult.IsSuccessful ? ytResult.Value : new List<YouTubeVideo>(),
            Udemy = udemyResult.IsSuccessful ? udemyResult.Value : new UdemyCourseListResponse(),
        };

        if (!ytResult.IsSuccessful)
            response.Errors.Add($"YouTube: {ytResult.FailureMessage}");

        if (!udemyResult.IsSuccessful)
            response.Errors.Add($"Udemy: {udemyResult.FailureMessage}");

        return Ok(response);
    }
}