namespace RibbitReels.Data.DTOs;

public class BranchResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<LeafResponse> Leaves { get; set; } = new();
}