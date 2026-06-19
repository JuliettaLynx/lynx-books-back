using System.ComponentModel.DataAnnotations;

namespace LynxBooks.Backend.Models;

public class User
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string DisplayName { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string? Avatar { get; set; }
    public string? OriginalAvatar { get; set; }
    public int DailyGoal { get; set; } = 50;
    public bool IsGoogleAccount { get; set; } = false;
    public bool IsLibraryPublic { get; set; } = false;
    public bool IsWishlistPublic { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<RefreshToken> RefreshTokens { get; set; } = new();
    public List<Book> Books { get; set; } = new();
    public List<Session> Sessions { get; set; } = new();
    public List<Wishlist> Wishlists { get; set; } = new();
    public List<Subscription> SubscriptionsAsSubscriber { get; set; } = new();
    public List<Subscription> SubscriptionsAsTarget { get; set; } = new();
}