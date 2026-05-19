namespace LynxBooks.Backend.DTOs.Users;

public class UpdateAvatarRequest
{
    public string? Avatar { get; set; }       // Base64
    public string? OriginalAvatar { get; set; } // Base64
}