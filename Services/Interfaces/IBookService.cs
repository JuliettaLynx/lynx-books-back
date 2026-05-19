using LynxBooks.Backend.DTOs.Books;

namespace LynxBooks.Backend.Services.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetUserBooksAsync(string userId, string? search, string? filter, string? sort);
    Task<BookDto?> GetBookByIdAsync(string bookId, string userId);
    Task<BookDto> CreateBookAsync(string userId, CreateBookRequest request);
    Task<bool> UpdateBookAsync(string bookId, string userId, UpdateBookRequest request);
    Task<bool> DeleteBookAsync(string bookId, string userId);
    Task<bool> ToggleFavoriteAsync(string bookId, string userId);
}