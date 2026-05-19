namespace LynxBooks.Backend.DTOs.Books;

public class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string Format { get; set; } = "бумажная";
    public string Status { get; set; } = "не прочитано";
    public int Rating { get; set; } = 0;
    public string? Description { get; set; }
    public string? Cover { get; set; }
    public string? OriginalCover { get; set; }
    public bool IsFavorite { get; set; } = false;
}