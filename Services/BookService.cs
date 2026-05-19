using LynxBooks.Backend.Data;
using LynxBooks.Backend.DTOs.Books;
using LynxBooks.Backend.Models;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LynxBooks.Backend.Services;

public class BookService : IBookService
{
    private readonly AppDbContext _context;

    public BookService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookDto>> GetUserBooksAsync(string userId, string? search, string? filter, string? sort)
    {
        var query = _context.Books.Where(b => b.UserId == userId).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(search) ||
                                    (b.Author != null && b.Author.ToLower().Contains(search)));
        }

        if (!string.IsNullOrEmpty(filter))
        {
            query = filter switch
            {
                "favorite" => query.Where(b => b.IsFavorite),
                "finished" => query.Where(b => b.Status == "прочитано"),
                "unfinished" => query.Where(b => b.Status == "не прочитано"),
                "abandoned" => query.Where(b => b.Status == "брошено"),
                _ => query
            };
        }

        query = sort switch
        {
            "title_asc" => query.OrderBy(b => b.Title),
            "title_desc" => query.OrderByDescending(b => b.Title),
            "author_asc" => query.OrderBy(b => b.Author),
            "author_desc" => query.OrderByDescending(b => b.Author),
            _ => query.OrderByDescending(b => b.CreatedAt)
        };

        var books = await query.ToListAsync();
        return books.Select(MapToDto);
    }

    public async Task<BookDto?> GetBookByIdAsync(string bookId, string userId)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.UserId == userId);
        return book == null ? null : MapToDto(book);
    }

    public async Task<BookDto> CreateBookAsync(string userId, CreateBookRequest request)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Publisher = request.Publisher,
            Format = request.Format,
            Status = request.Status,
            Rating = request.Rating,
            Description = request.Description,
            Cover = request.Cover,
            OriginalCover = request.OriginalCover,
            IsFavorite = request.IsFavorite,
            UserId = userId
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return MapToDto(book);
    }

    public async Task<bool> UpdateBookAsync(string bookId, string userId, UpdateBookRequest request)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.UserId == userId);
        if (book == null) return false;

        book.Title = request.Title;
        book.Author = request.Author;
        book.Publisher = request.Publisher;
        book.Format = request.Format;
        book.Status = request.Status;
        book.Rating = request.Rating;
        book.Description = request.Description;
        book.Cover = request.Cover;
        book.OriginalCover = request.OriginalCover;
        book.IsFavorite = request.IsFavorite;
        book.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteBookAsync(string bookId, string userId)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.UserId == userId);
        if (book == null) return false;
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleFavoriteAsync(string bookId, string userId)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.UserId == userId);
        if (book == null) return false;
        book.IsFavorite = !book.IsFavorite;
        book.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static BookDto MapToDto(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Publisher = book.Publisher,
        Format = book.Format,
        Status = book.Status,
        Rating = book.Rating,
        Description = book.Description,
        Cover = book.Cover,
        OriginalCover = book.OriginalCover,
        IsFavorite = book.IsFavorite,
        CreatedAt = book.CreatedAt,
        UpdatedAt = book.UpdatedAt
    };
}