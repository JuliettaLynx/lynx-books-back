using LynxBooks.Backend.Data;
using LynxBooks.Backend.DTOs.Auth;
using LynxBooks.Backend.Models;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LynxBooks.Backend.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, JwtService jwtService, IConfiguration configuration)
    {
        _context = context;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("User with this email already exists");

        var user = new User
        {
            Email = request.Email,
            DisplayName = request.DisplayName ?? request.Email.Split('@')[0],
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsGoogleAccount = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || user.IsGoogleAccount)
            throw new UnauthorizedAccessException("Invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password");

        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);

        if (token == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        var user = token.User;
        return await GenerateAuthResponse(user);
    }

    public async Task<AuthResponse> GoogleSignInAsync(GoogleSignInRequest request)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(request.Credential);

            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var picture = jwtToken.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("Invalid Google token");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // Скачиваем аватар и конвертируем в Base64
                string? avatarBase64 = null;
                if (!string.IsNullOrEmpty(picture))
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        var imageBytes = await httpClient.GetByteArrayAsync(picture);
                        avatarBase64 = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to download avatar: {ex.Message}");
                        // Продолжаем без аватара
                    }
                }

                user = new User
                {
                    Email = email,
                    DisplayName = name ?? email.Split('@')[0],
                    Avatar = avatarBase64,        // Base64 вместо URL
                    OriginalAvatar = avatarBase64, // Base64 вместо URL
                    IsGoogleAccount = true,
                    PasswordHash = null
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else if (!user.IsGoogleAccount)
            {
                throw new UnauthorizedAccessException("This email is registered with password.");
            }
            else
            {
                // Обновляем аватар если изменился
                if (!string.IsNullOrEmpty(picture) && user.Avatar != picture)
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        var imageBytes = await httpClient.GetByteArrayAsync(picture);
                        var avatarBase64 = $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
                        
                        user.Avatar = avatarBase64;
                        user.OriginalAvatar = avatarBase64;
                        user.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to update avatar: {ex.Message}");
                    }
                }
            }

            return await GenerateAuthResponse(user);
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"Invalid Google token: {ex.Message}");
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : MapToDto(user);
    }

    private async Task<AuthResponse> GenerateAuthResponse(User user)
    {
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Удаляем старые refresh токены (опционально)
        var oldTokens = _context.RefreshTokens.Where(rt => rt.UserId == user.Id && rt.ExpiresAt < DateTime.UtcNow);
        _context.RefreshTokens.RemoveRange(oldTokens);

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        });
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = MapToDto(user)
        };
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

    public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.IsGoogleAccount) return false;

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Неверный текущий пароль");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAccountAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.Books)
            .Include(u => u.Sessions)
            .Include(u => u.Wishlists)
            .Include(u => u.SubscriptionsAsSubscriber)
            .Include(u => u.SubscriptionsAsTarget)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null) return false;

        // Все каскадные удаления теперь работают благодаря OnDelete(Cascade)
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}