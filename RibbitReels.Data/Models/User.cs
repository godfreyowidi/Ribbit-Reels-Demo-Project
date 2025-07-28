using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RibbitReels.Data.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        // Shown in the UI, from OAuth or user input
        public string DisplayName { get; set; } = null!;

        // Optional avatar or profile picture from OAuth providers
        public string? AvatarUrl { get; set; }

        // The OAuth provider name (e.g., Google, Facebook)
        public string AuthProvider { get; set; } = "local"; // fallback for future local auth

        // The provider-specific user ID (e.g., Google's sub)
        public string? ProviderUserId { get; set; }

        // Either "User", "Admin", etc.
        [Required]
        public string Role { get; set; } = "User";

        // Used only if supporting email/password login later
        public string? PasswordHash { get; set; }

        // Navigation properties
        public ICollection<LearningProgress> Progress { get; set; } = new List<LearningProgress>();
        public ICollection<UserBranchAssignment> AssignedBranches { get; set; } = new List<UserBranchAssignment>();
    }
}
