namespace RibbitReels.Data.DTOs;

public class YouTubeVideo
{
    public string VideoId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
}
