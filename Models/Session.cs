using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LynxBooks.Backend.Models;

public class Session
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BookId { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string Color { get; set; } = "#3B82F6";
    public DateTime Date { get; set; }
    public DateTime? StartDate { get; set; }
    public int? StartPage { get; set; }
    public int? EndPage { get; set; }
    public int PagesRead { get; set; }
    public bool FinishedBook { get; set; } = false;
    public int Rating { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = string.Empty;
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; } = null!;
}