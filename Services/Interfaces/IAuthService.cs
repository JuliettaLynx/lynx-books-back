using LynxBooks.Backend.DTOs.Auth;

namespace LynxBooks.Backend.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<bool> DeleteAccountAsync(string userId);
    Task<AuthResponse> GoogleSignInAsync(GoogleSignInRequest request);
}