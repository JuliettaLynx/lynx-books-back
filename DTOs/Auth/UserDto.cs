namespace LynxBooks.Backend.DTOs.Auth;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Avatar { get; set; }
    public string? OriginalAvatar { get; set; }
    public int DailyGoal { get; set; }
    public bool IsGoogleAccount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}