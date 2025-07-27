namespace RibbitReels.Api.DTOs;

public class CreateLeafRequest
{
    public Guid BranchId { get; set; }
    public string Title { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public int Order { get; set; }
}
