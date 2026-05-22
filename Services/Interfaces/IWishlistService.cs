using LynxBooks.Backend.DTOs.Wishlist;

namespace LynxBooks.Backend.Services.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(string userId, string? sortBy, int? priorityFilter);
    Task<WishlistDto?> GetWishlistBookByIdAsync(string id, string userId);
    Task<WishlistDto> CreateWishlistBookAsync(string userId, CreateWishlistRequest request);
    Task<bool> UpdateWishlistBookAsync(string id, string userId, UpdateWishlistRequest request);
    Task<bool> DeleteWishlistBookAsync(string id, string userId, string reason);
}