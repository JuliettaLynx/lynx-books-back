using LynxBooks.Backend.DTOs.Users;
using LynxBooks.Backend.DTOs.Auth;
using LynxBooks.Backend.Models;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LynxBooks.Backend.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private string GetUserId() =>
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException();

    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var userId = GetUserId();
        var profile = await _userService.GetUserProfileAsync(userId);
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var userId = GetUserId();
        var success = await _userService.UpdateProfileAsync(userId, request);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut("avatar")]
    public async Task<IActionResult> UpdateAvatar(UpdateAvatarRequest request)
    {
        var userId = GetUserId();
        var success = await _userService.UpdateAvatarAsync(userId, request);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut("daily-goal")]
    public async Task<IActionResult> SetDailyGoal([FromBody] int dailyGoal)
    {
        var userId = GetUserId();
        var success = await _userService.SetDailyGoalAsync(userId, dailyGoal);
        if (!success) return NotFound();
        return NoContent();
    }
}