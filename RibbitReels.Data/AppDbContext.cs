using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RibbitReels.Data.Models;

namespace RibbitReels.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Leaf> Leaves => Set<Leaf>();
    public DbSet<UserProgress> UserProgress => Set<UserProgress>();
    public DbSet<AssignedBranch> AssignedBranches => Set<AssignedBranch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Might consider configuring JSON serialization for CompletedLeafIds
        modelBuilder.Entity<UserProgress>()
            .Property(up => up.CompletedLeafIds)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(Guid.Parse)
                    .ToList()
            )
            .Metadata.SetValueComparer(
                new ValueComparer<List<Guid>>(
                    (c1, c2) => (c1 ?? new List<Guid>()).SequenceEqual(c2 ?? new List<Guid>()),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );
    }
}