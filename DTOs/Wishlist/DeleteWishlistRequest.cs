namespace LynxBooks.Backend.DTOs.Wishlist;

public class DeleteWishlistRequest
{
    public string Reason { get; set; } = "not_relevant"; // "not_relevant" или "purchased"
}