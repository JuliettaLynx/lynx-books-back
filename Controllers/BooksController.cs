using LynxBooks.Backend.DTOs.Books;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LynxBooks.Backend.Controllers;

[ApiController]
[Route("api/books")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    private string GetUserId() =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(
        [FromQuery] string? search,
        [FromQuery] string? filter,
        [FromQuery] string? sort)
    {
        var userId = GetUserId();
        var books = await _bookService.GetUserBooksAsync(userId, search, filter, sort);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(string id)
    {
        var userId = GetUserId();
        var book = await _bookService.GetBookByIdAsync(id, userId);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookRequest request)
    {
        var userId = GetUserId();
        var book = await _bookService.CreateBookAsync(userId, request);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(string id, UpdateBookRequest request)
    {
        var userId = GetUserId();
        var success = await _bookService.UpdateBookAsync(id, userId, request);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(string id)
    {
        var userId = GetUserId();
        var success = await _bookService.DeleteBookAsync(id, userId);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/favorite")]
    public async Task<IActionResult> ToggleFavorite(string id)
    {
        var userId = GetUserId();
        var success = await _bookService.ToggleFavoriteAsync(id, userId);
        if (!success) return NotFound();
        return NoContent();
    }
}