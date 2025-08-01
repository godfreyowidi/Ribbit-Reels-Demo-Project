using System.ComponentModel.DataAnnotations;
using RibbitReels.Data.Models;

namespace RibbitReels.Data.DTOs;
public class UpdateUserRequest
{
    [EmailAddress]
    public string? Email { get; set; }

    public string? DisplayName { get; set; }

    [Url]
    public string? AvatarUrl { get; set; }

    public UserRole? Role { get; set; }
}
