using LynxBooks.Backend.Data;
using LynxBooks.Backend.DTOs.Users;
using LynxBooks.Backend.DTOs.Auth;
using LynxBooks.Backend.Models;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LynxBooks.Backend.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> GetUserProfileAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : MapToDto(user);
    }

    public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (!string.IsNullOrWhiteSpace(request.DisplayName))
            user.DisplayName = request.DisplayName;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAvatarAsync(string userId, UpdateAvatarRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Avatar = request.Avatar;
        user.OriginalAvatar = request.OriginalAvatar;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetDailyGoalAsync(string userId, int dailyGoal)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.DailyGoal = dailyGoal;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        Avatar = user.Avatar,
        OriginalAvatar = user.OriginalAvatar,
        DailyGoal = user.DailyGoal,
        IsGoogleAccount = user.IsGoogleAccount,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };
}