namespace RibbitReels.Data.DTOs;

public class LeafResponse
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string Title { get; set; } = null!;

    public string? Text { get; set; }

    public string VideoUrl { get; set; } = null!;
    public string? ThumbnailUrl { get; set; }

    public int Order { get; set; }
    public string Source { get; set; } = "Manual";
    public string Status { get; set; } = "Published";

    public DateTime CreatedAt { get; set; }
}
