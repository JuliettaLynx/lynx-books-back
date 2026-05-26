namespace LynxBooks.Backend.DTOs.Community;

public class UserSearchResultDto
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsLibraryPublic { get; set; }
    public bool IsWishlistPublic { get; set; }
    public string? Avatar { get; set; }
}