namespace LynxBooks.Backend.DTOs.Wishlist;

public class UpdateWishlistRequest
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Cover { get; set; }
    public string? OriginalCover { get; set; }
    public string? Binding { get; set; }
    public string? Note { get; set; }
    public string? Description { get; set; }
    public int? Priority { get; set; }
}