namespace LynxBooks.Backend.DTOs.Books;

public class UpdateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string Format { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Description { get; set; }
    public string? Cover { get; set; }
    public string? OriginalCover { get; set; }
    public bool IsFavorite { get; set; }
}