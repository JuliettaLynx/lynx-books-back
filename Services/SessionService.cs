using LynxBooks.Backend.Data;
using LynxBooks.Backend.DTOs.Sessions;
using LynxBooks.Backend.Models;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LynxBooks.Backend.Services;

public class SessionService : ISessionService
{
    private readonly AppDbContext _context;

    public SessionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SessionDto>> GetUserSessionsAsync(string userId, DateTime? from, DateTime? to, string? bookId)
    {
        var query = _context.Sessions.Where(s => s.UserId == userId);
        if (from.HasValue) query = query.Where(s => s.Date >= from.Value);
        if (to.HasValue) query = query.Where(s => s.Date <= to.Value);
        if (!string.IsNullOrEmpty(bookId)) query = query.Where(s => s.BookId == bookId);
        var sessions = await query.OrderByDescending(s => s.Date).ToListAsync();
        return sessions.Select(MapToDto);
    }

    public async Task<SessionDto?> GetSessionByIdAsync(string sessionId, string userId)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
        return session == null ? null : MapToDto(session);
    }

    public async Task<SessionDto> CreateSessionAsync(string userId, CreateSessionRequest request)
    {
        var session = new Session
        {
            BookId = request.BookId,
            BookTitle = request.BookTitle,
            Color = request.Color,
            Date = request.Date,
            StartDate = request.StartDate,
            StartPage = request.StartPage,
            EndPage = request.EndPage,
            PagesRead = request.PagesRead,
            FinishedBook = request.FinishedBook,
            Rating = request.Rating,
            UserId = userId
        };
        _context.Sessions.Add(session);
        
        if (request.FinishedBook)
        {
            var book = await _context.Books.FindAsync(request.BookId);
            if (book != null && book.UserId == userId)
            {
                book.Status = "прочитано";
                book.Rating = request.Rating;
                book.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
        return MapToDto(session);
    }

    public async Task<bool> UpdateSessionAsync(string sessionId, string userId, UpdateSessionRequest request)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
        if (session == null) return false;
        if (request.Color != null) session.Color = request.Color;
        if (request.Date.HasValue) session.Date = request.Date.Value;
        if (request.StartDate.HasValue) session.StartDate = request.StartDate;
        if (request.StartPage.HasValue) session.StartPage = request.StartPage;
        if (request.EndPage.HasValue) session.EndPage = request.EndPage;
        if (request.PagesRead.HasValue) session.PagesRead = request.PagesRead.Value;
        if (request.FinishedBook.HasValue) session.FinishedBook = request.FinishedBook.Value;
        if (request.Rating.HasValue) session.Rating = request.Rating.Value;
        session.UpdatedAt = DateTime.UtcNow;
        if (request.FinishedBook == true)
        {
            var book = await _context.Books.FindAsync(session.BookId);
            if (book != null && book.UserId == userId)
            {
                book.Status = "прочитано";
                book.Rating = request.Rating ?? book.Rating;
                book.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSessionAsync(string sessionId, string userId)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
        if (session == null) return false;
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }

    private static SessionDto MapToDto(Session s) => new()
    {
        Id = s.Id,
        BookId = s.BookId,
        BookTitle = s.BookTitle,
        Color = s.Color,
        Date = s.Date,
        StartDate = s.StartDate,
        StartPage = s.StartPage,
        EndPage = s.EndPage,
        PagesRead = s.PagesRead,
        FinishedBook = s.FinishedBook,
        Rating = s.Rating,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}