namespace LynxBooks.Backend.DTOs.Sessions;

public class CreateSessionRequest
{
    public string BookId { get; set; } = string.Empty;
    public string BookTitle { get; set; } = string.Empty;
    public string Color { get; set; } = "#FF0000";
    public DateTime Date { get; set; }
    public DateTime? StartDate { get; set; }
    public int? StartPage { get; set; }
    public int? EndPage { get; set; }
    public int PagesRead { get; set; }
    public bool FinishedBook { get; set; } = false;
    public int Rating { get; set; } = 0;
}