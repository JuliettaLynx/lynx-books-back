namespace LynxBooks.Backend.DTOs.Sessions;

public class UpdateSessionRequest
{
    public string? Color { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? StartDate { get; set; }
    public int? StartPage { get; set; }
    public int? EndPage { get; set; }
    public int? PagesRead { get; set; }
    public bool? FinishedBook { get; set; }
    public int? Rating { get; set; }
}