using LynxBooks.Backend.DTOs.Sessions;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LynxBooks.Backend.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    private string GetUserId() =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetSessions(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? bookId)
    {
        var userId = GetUserId();
        var sessions = await _sessionService.GetUserSessionsAsync(userId, from, to, bookId);
        return Ok(sessions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SessionDto>> GetSession(string id)
    {
        var userId = GetUserId();
        var session = await _sessionService.GetSessionByIdAsync(id, userId);
        if (session == null) return NotFound();
        return Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<SessionDto>> CreateSession(CreateSessionRequest request)
    {
        var userId = GetUserId();
        var session = await _sessionService.CreateSessionAsync(userId, request);
        return Ok(session);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSession(string id, UpdateSessionRequest request)
    {
        var userId = GetUserId();
        var success = await _sessionService.UpdateSessionAsync(id, userId, request);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSession(string id)
    {
        var userId = GetUserId();
        var success = await _sessionService.DeleteSessionAsync(id, userId);
        if (!success) return NotFound();
        return NoContent();
    }
}