using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RibbitReels.Data.Models
{
    public class Leaf
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; } = null!;

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string VideoUrl { get; set; } = null!;

        public int Order { get; set; }
    }
}
