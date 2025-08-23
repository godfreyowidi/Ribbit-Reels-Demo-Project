using Microsoft.AspNetCore.Http;

namespace RibbitReels.Api.DTOs;

public abstract class CreateLeafRequestBase
{
    public Guid BranchId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Order { get; set; }
}

public class CreateManualLeafRequest : CreateLeafRequestBase
{
    public IFormFile? VideoFile { get; set; }
}

public class CreateYouTubeLeafRequest : CreateLeafRequestBase
{
    public string VideoId { get; set; } = null!;
    public string? ThumbnailUrl { get; set; }
}
