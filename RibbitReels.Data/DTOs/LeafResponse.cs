namespace RibbitReels.Data.DTOs;

public class LeafResponse
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public string Title { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public int Order { get; set; }
}
