using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RibbitReels.Data.Models;

public class Leaf
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid BranchId { get; set; }

    [ForeignKey(nameof(BranchId))]
    public Branch Branch { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = null!;

    // TY
    [MaxLength(50)]
    public string? YouTubeVideoId { get; set; }

    // manual uploads
    public byte[]? VideoData { get; set; }
    [MaxLength(255)]
    public string? VideoFileName { get; set; }
    [MaxLength(50)]
    public string? VideoContentType { get; set; }

    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }

    public int Order { get; set; }
    [MaxLength(50)]
    public string Source { get; set; } = "Manual";
    [MaxLength(20)]
    public string Status { get; set; } = "Published";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


