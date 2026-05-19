namespace LynxBooks.Backend.DTOs.Sessions;

public class SessionDto
{
    public string Id { get; set; } = string.Empty;
    public string BookId { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public DateTime? StartDate { get; set; }
    public int? StartPage { get; set; }
    public int? EndPage { get; set; }
    public int PagesRead { get; set; }
    public bool FinishedBook { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}