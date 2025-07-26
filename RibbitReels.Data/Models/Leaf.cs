namespace RibbitReels.Data.Models;


public class Leaf
{
    public Guid Id { get; set; }
    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public string VideoUrl { get; set; } = null!;
    public string Text { get; set; } = null!;
    public int Order { get; set; }
}