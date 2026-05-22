using LynxBooks.Backend.DTOs.Wishlist;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LynxBooks.Backend.Controllers;

[ApiController]
[Route("api/wishlist")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private string GetUserId() =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishlistDto>>> GetWishlist(
        [FromQuery] string? sortBy,
        [FromQuery] int? priority)
    {
        var userId = GetUserId();
        var books = await _wishlistService.GetUserWishlistAsync(userId, sortBy, priority);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WishlistDto>> GetWishlistBook(string id)
    {
        var userId = GetUserId();
        var book = await _wishlistService.GetWishlistBookByIdAsync(id, userId);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<WishlistDto>> CreateWishlistBook(CreateWishlistRequest request)
    {
        var userId = GetUserId();
        var book = await _wishlistService.CreateWishlistBookAsync(userId, request);
        return CreatedAtAction(nameof(GetWishlistBook), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWishlistBook(string id, UpdateWishlistRequest request)
    {
        var userId = GetUserId();
        var success = await _wishlistService.UpdateWishlistBookAsync(id, userId, request);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWishlistBook(string id, [FromQuery] string reason = "not_relevant")
    {
        var userId = GetUserId();
        var success = await _wishlistService.DeleteWishlistBookAsync(id, userId, reason);
        if (!success) return NotFound();
        return Ok(new { message = reason == "purchased" 
            ? "Книга перемещена в библиотеку" 
            : "Книга удалена из виш-листа" });
    }
}