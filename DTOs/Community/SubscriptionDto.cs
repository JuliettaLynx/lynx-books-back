namespace LynxBooks.Backend.DTOs.Community;

public class SubscriptionDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool HasLibraryAccess { get; set; }   // подписка + публичность
    public bool HasWishlistAccess { get; set; }
}