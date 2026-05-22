namespace LynxBooks.Backend.DTOs.Wishlist;

public class CreateWishlistRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Cover { get; set; }
    public string? OriginalCover { get; set; }
    public string Binding { get; set; } = "твердый";
    public int Priority { get; set; } = 1;
}