namespace RibbitReels.Data.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;

    public ICollection<LearningProgress> Progress { get; set; } = new List<LearningProgress>();
    public ICollection<UserBranchAssignment> AssignedBranches { get; set; } = new List<UserBranchAssignment>();
}