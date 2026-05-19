using LynxBooks.Backend.DTOs.Users;
using LynxBooks.Backend.DTOs.Auth;

namespace LynxBooks.Backend.Services.Interfaces;

public interface IUserService{
    Task<UserDto?> GetUserProfileAsync(string userId);

    Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request);

    Task<bool> UpdateAvatarAsync(string userId, UpdateAvatarRequest request);

    Task<bool> SetDailyGoalAsync(string userId, int dailyGoal);
}