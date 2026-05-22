using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LynxBooks.Backend.Models;

public class Wishlist
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Cover { get; set; }
    public string? OriginalCover { get; set; }
    public string Binding { get; set; } = "твердый"; // "твердый" или "мягкий"
    public int Priority { get; set; } = 1; // от 1 (низкий) до 6 (высокий)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string UserId { get; set; } = string.Empty;
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}