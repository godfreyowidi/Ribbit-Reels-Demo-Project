namespace RibbitReels.Data.Models;

public class Branch
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<Leaf> Leafs { get; set; } = new List<Leaf>();
}