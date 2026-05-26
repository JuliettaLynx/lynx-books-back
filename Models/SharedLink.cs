using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LynxBooks.Backend.Models;

public class SharedLink
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool GrantsLibraryAccess { get; set; } = false;
    public bool GrantsWishlistAccess { get; set; } = false;
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
    public bool IsUsed { get; set; } = false; // одноразовая ссылка
    
    public string UserId { get; set; } = string.Empty;
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}