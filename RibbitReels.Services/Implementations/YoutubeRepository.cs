using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RibbitReels.Data.Configs;
using RibbitReels.Data.DTOs;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Implementations;

public class YouTubeRepository
{
    private readonly YouTubeConfiguration _youTubeConfig;
    private readonly HttpClient _httpClient;

    public YouTubeRepository(
        IOptions<YouTubeConfiguration> options,
        HttpClient httpClient)
    {
        _youTubeConfig = options.Value;
        _httpClient = httpClient;
    }


    public async Task<OperationResult<List<YouTubeVideo>>> SearchVideosAsync(string subject, int maxResults = 5)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(subject))
                return OperationResult<List<YouTubeVideo>>.Fail("Subject cannot be empty.", HttpStatusCode.BadRequest);

            var url = $"{_youTubeConfig.BaseUrl}/search" +
                      $"?q={Uri.EscapeDataString(subject)}" +
                      $"&part=snippet" +
                      $"&type=video" +
                      $"&videoDuration=short" +
                      $"&maxResults={maxResults}" +
                      $"&key={_youTubeConfig.ApiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return OperationResult<List<YouTubeVideo>>.Fail($"YouTube API error: {response.StatusCode}", HttpStatusCode.BadGateway);

            using var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(stream);

            var items = json.RootElement.GetProperty("items");
            var results = new List<YouTubeVideo>();

            foreach (var item in items.EnumerateArray())
            {
                var videoId = item.GetProperty("id").GetProperty("videoId").GetString();
                var snippet = item.GetProperty("snippet");

                results.Add(new YouTubeVideo
                {
                    VideoId = videoId!,
                    Title = snippet.GetProperty("title").GetString() ?? "Untitled",
                    Description = snippet.GetProperty("description").GetString() ?? string.Empty,
                    ThumbnailUrl = snippet.GetProperty("thumbnails").GetProperty("high").GetProperty("url").GetString()
                });
            }

            return OperationResult<List<YouTubeVideo>>.Success(results);
        }
        catch (Exception ex)
        {
            return OperationResult<List<YouTubeVideo>>.Fail(ex, "Failed to fetch tutorials from YouTube.");
        }
    }

    public async Task<OperationResult<YouTubeVideo>> GetVideoDetailsAsync(string videoId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(videoId))
                return OperationResult<YouTubeVideo>.Fail("VideoId cannot be empty.", HttpStatusCode.BadRequest);

            var url = $"{_youTubeConfig.BaseUrl}/videos" +
                      $"?id={Uri.EscapeDataString(videoId)}" +
                      $"&part=snippet" +
                      $"&key={_youTubeConfig.ApiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return OperationResult<YouTubeVideo>.Fail($"YouTube API error: {response.StatusCode}", HttpStatusCode.BadGateway);

            using var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(stream);

            var items = json.RootElement.GetProperty("items");
            if (items.GetArrayLength() == 0)
                return OperationResult<YouTubeVideo>.Fail("Video not found.", HttpStatusCode.NotFound);

            var snippet = items[0].GetProperty("snippet");

            var video = new YouTubeVideo
            {
                VideoId = videoId,
                Title = snippet.GetProperty("title").GetString() ?? "Untitled",
                Description = snippet.GetProperty("description").GetString() ?? string.Empty,
                ThumbnailUrl = snippet.GetProperty("thumbnails").GetProperty("high").GetProperty("url").GetString()
            };

            return OperationResult<YouTubeVideo>.Success(video);
        }
        catch (Exception ex)
        {
            return OperationResult<YouTubeVideo>.Fail(ex, "Failed to fetch video details from YouTube.");
        }
    }
}