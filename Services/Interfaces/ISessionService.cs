using LynxBooks.Backend.DTOs.Sessions;

namespace LynxBooks.Backend.Services.Interfaces;

public interface ISessionService
{
    Task<IEnumerable<SessionDto>> GetUserSessionsAsync(string userId, DateTime? from, DateTime? to, string? bookId);
    Task<SessionDto?> GetSessionByIdAsync(string sessionId, string userId);
    Task<SessionDto> CreateSessionAsync(string userId, CreateSessionRequest request);
    Task<bool> UpdateSessionAsync(string sessionId, string userId, UpdateSessionRequest request);
    Task<bool> DeleteSessionAsync(string sessionId, string userId);
}