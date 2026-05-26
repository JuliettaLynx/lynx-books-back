using LynxBooks.Backend.DTOs.Community;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LynxBooks.Backend.Controllers;

[ApiController]
[Route("api/community")]
[Authorize]
public class CommunityController : ControllerBase
{
    private readonly ICommunityService _communityService;
    private readonly IBookService _bookService;
    private readonly IWishlistService _wishlistService;
    private readonly ILogger<CommunityController> _logger;

    public CommunityController(
    ICommunityService communityService,
    IBookService bookService,
    IWishlistService wishlistService,
    ILogger<CommunityController> logger)
    {
        _communityService = communityService;
        _bookService = bookService;
        _wishlistService = wishlistService;
        _logger = logger;
    }

    private string GetUserId() =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException();

    [HttpGet("privacy")]
    public async Task<ActionResult<PrivacySettingsDto>> GetPrivacy()
    {
        var settings = await _communityService.GetPrivacySettingsAsync(GetUserId());
        return Ok(settings);
    }

    [HttpPut("privacy")]
    public async Task<IActionResult> UpdatePrivacy(PrivacySettingsDto settings)
    {
        await _communityService.UpdatePrivacySettingsAsync(GetUserId(), settings);
        return NoContent();
    }

    [HttpPost("share")]
    public async Task<ActionResult<SharedLinkResponseDto>> GenerateShareLink([FromBody] GenerateShareRequestDto request)
    {
        var link = await _communityService.GenerateSharedLinkAsync(GetUserId(), request.Library, request.Wishlist);
        return Ok(link);
    }

    [HttpGet("validate-share")]
    public async Task<ActionResult<ValidateShareResponseDto>> ValidateShare([FromQuery] string token)
    {
        var result = await _communityService.ValidateSharedLinkAsync(token);
        return Ok(result);
    }

    [HttpGet("search-users")]
    public async Task<ActionResult<IEnumerable<UserSearchResultDto>>> SearchUsers([FromQuery] string query)
    {
        var users = await _communityService.SearchUsersAsync(query, GetUserId());
        return Ok(users);
    }

    [HttpPost("subscribe/{targetUserId}")]
    public async Task<IActionResult> Subscribe(string targetUserId, [FromQuery] string? listType)
    {
        await _communityService.SubscribeAsync(GetUserId(), targetUserId, listType);
        return NoContent();
    }

    [HttpDelete("subscribe/{targetUserId}")]
    public async Task<IActionResult> Unsubscribe(string targetUserId, [FromQuery] string? listType)
    {
        await _communityService.UnsubscribeAsync(GetUserId(), targetUserId, listType);
        return NoContent();
    }

    [HttpGet("subscriptions")]
    public async Task<ActionResult<IEnumerable<SubscriptionDto>>> GetSubscriptions()
    {
        var subs = await _communityService.GetSubscriptionsAsync(GetUserId());
        return Ok(subs);
    }

    [HttpGet("users/{userId}/library")]
    public async Task<IActionResult> GetUserLibrary(string userId, [FromQuery] string? sharedToken)
    {
        var currentUserId = GetUserId();
        _logger.LogInformation("GetUserLibrary: currentUserId={0}, targetUserId={1}, sharedToken={2}", currentUserId, userId, sharedToken);
        var canAccess = await _communityService.CanAccessListAsync(currentUserId, userId, "library", sharedToken);
        if (!canAccess) return Forbid("No access to this library");

        var books = await _bookService.GetUserBooksAsync(userId, null, null, null);
        _logger.LogInformation("Books count: {0}", books.Count());
        return Ok(books);
    }

    [HttpGet("users/{userId}/wishlist")]
    public async Task<IActionResult> GetUserWishlist(string userId, [FromQuery] string? sharedToken)
    {
        var currentUserId = GetUserId();
        var canAccess = await _communityService.CanAccessListAsync(currentUserId, userId, "wishlist", sharedToken);
        if (!canAccess) return Forbid("No access to this wishlist");

        var wishlist = await _wishlistService.GetUserWishlistAsync(userId, null, null);
        return Ok(wishlist);
    }
}