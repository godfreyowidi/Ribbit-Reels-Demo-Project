namespace RibbitReels.Data.Models;

public class AssignedBranch
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid AssignedByManagerId { get; set; }
}