namespace RibbitReels.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;

    public ICollection<UserProgress> Progress { get; set; } = new List<UserProgress>();
    public ICollection<AssignedBranch> AssignedBranches { get; set; } = new List<AssignedBranch>();
}