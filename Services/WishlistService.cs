using LynxBooks.Backend.Data;
using LynxBooks.Backend.DTOs.Wishlist;
using LynxBooks.Backend.Models;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LynxBooks.Backend.Services;

public class WishlistService : IWishlistService
{
    private readonly AppDbContext _context;

    public WishlistService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(string userId, string? sortBy, int? priorityFilter)
    {
        var query = _context.Wishlists.Where(w => w.UserId == userId);

        if (priorityFilter.HasValue && priorityFilter >= 1 && priorityFilter <= 6)
        {
            query = query.Where(w => w.Priority == priorityFilter.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "title_asc" => query.OrderBy(w => w.Title),
            "title_desc" => query.OrderByDescending(w => w.Title),
            "author_asc" => query.OrderBy(w => w.Author),
            "author_desc" => query.OrderByDescending(w => w.Author),
            "priority_asc" => query.OrderBy(w => w.Priority),
            "priority_desc" => query.OrderByDescending(w => w.Priority),
            _ => query.OrderByDescending(w => w.CreatedAt) // по умолчанию новые сверху
        };

        var books = await query.ToListAsync();
        return books.Select(MapToDto);
    }

    public async Task<WishlistDto?> GetWishlistBookByIdAsync(string id, string userId)
    {
        var book = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        return book == null ? null : MapToDto(book);
    }

    public async Task<WishlistDto> CreateWishlistBookAsync(string userId, CreateWishlistRequest request)
    {
        var book = new Wishlist
        {
            Title = request.Title,
            Author = request.Author,
            Publisher = request.Publisher,
            Cover = request.Cover,
            OriginalCover = request.OriginalCover,
            Binding = request.Binding,
            Priority = request.Priority,
            UserId = userId
        };

        _context.Wishlists.Add(book);
        await _context.SaveChangesAsync();
        return MapToDto(book);
    }

    public async Task<bool> UpdateWishlistBookAsync(string id, string userId, UpdateWishlistRequest request)
    {
        var book = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (book == null) return false;

        if (!string.IsNullOrWhiteSpace(request.Title))
            book.Title = request.Title;
        if (!string.IsNullOrWhiteSpace(request.Author))
            book.Author = request.Author;
        if (!string.IsNullOrWhiteSpace(request.Publisher))
            book.Publisher = request.Publisher;
        book.Cover = request.Cover;
        book.OriginalCover = request.OriginalCover; 
        if (!string.IsNullOrWhiteSpace(request.Binding))
            book.Binding = request.Binding;
        if (request.Priority.HasValue && request.Priority.Value >= 1 && request.Priority.Value <= 6)
            book.Priority = request.Priority.Value;

        book.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWishlistBookAsync(string id, string userId, string reason)
    {
        var wishlistBook = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (wishlistBook == null) return false;

        if (reason == "purchased")
        {
            // Переносим в таблицу Books
            var newBook = new Book
            {
                Title = wishlistBook.Title,
                Author = wishlistBook.Author,
                Publisher = wishlistBook.Publisher,
                Cover = wishlistBook.Cover,
                OriginalCover = wishlistBook.OriginalCover,
                Format = "бумажная",
                Status = "не прочитано",
                Rating = 0,
                IsFavorite = false,
                UserId = userId
            };
            _context.Books.Add(newBook);
        }

        _context.Wishlists.Remove(wishlistBook);
        await _context.SaveChangesAsync();
        return true;
    }

    private static WishlistDto MapToDto(Wishlist book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Publisher = book.Publisher,
        Cover = book.Cover,
        OriginalCover = book.OriginalCover,
        Binding = book.Binding,
        Priority = book.Priority,
        CreatedAt = book.CreatedAt,
        UpdatedAt = book.UpdatedAt
    };
}