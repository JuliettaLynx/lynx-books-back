namespace LynxBooks.Backend.DTOs.Wishlist;

public class WishlistDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Cover { get; set; }
    public string? OriginalCover { get; set; }
    public string Binding { get; set; } = "твердый";
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}