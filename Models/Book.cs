using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LynxBooks.Backend.Models;

public class Book
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string Format { get; set; } = "бумажная"; // бумажная, электронная, аудио
    public string Status { get; set; } = "не прочитано";
    public int Rating { get; set; } = 0;
    public string? Review { get; set; }
    public string? Description { get; set; }
    public string? Cover { get; set; }        // Base64 превью (200x300)
    public string? OriginalCover { get; set; } // Base64 оригинал (не кэшируем офлайн)
    public bool IsFavorite { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = string.Empty;
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    public List<Session> Sessions { get; set; } = new();
}