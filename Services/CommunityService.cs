using LynxBooks.Backend.Data;
using LynxBooks.Backend.DTOs.Community;
using LynxBooks.Backend.Models;
using LynxBooks.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LynxBooks.Backend.Services;

public class CommunityService : ICommunityService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CommunityService> _logger;

    public CommunityService(AppDbContext context, ILogger<CommunityService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PrivacySettingsDto> GetPrivacySettingsAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException();
        return new PrivacySettingsDto
        {
            IsLibraryPublic = user.IsLibraryPublic,
            IsWishlistPublic = user.IsWishlistPublic
        };
    }

    public async Task UpdatePrivacySettingsAsync(string userId, PrivacySettingsDto settings)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException();
        user.IsLibraryPublic = settings.IsLibraryPublic;
        user.IsWishlistPublic = settings.IsWishlistPublic;
        await _context.SaveChangesAsync();
    }

    public async Task<SharedLinkResponseDto> GenerateSharedLinkAsync(string userId, bool grantLibrary, bool grantWishlist)
    {
        if (!grantLibrary && !grantWishlist)
            throw new ArgumentException("At least one list must be selected");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new UnauthorizedAccessException();

        var link = new SharedLink
        {
            UserId = userId,
            GrantsLibraryAccess = grantLibrary,
            GrantsWishlistAccess = grantWishlist,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        _context.SharedLinks.Add(link);
        await _context.SaveChangesAsync();

        var baseUrl = "http://localhost:5173"; // вынести в конфиг
        var fullLink = $"{baseUrl}/community?share={link.Token}";

        return new SharedLinkResponseDto
        {
            Link = fullLink,
            Token = link.Token,
            ExpiresAt = link.ExpiresAt
        };
    }

    public async Task<ValidateShareResponseDto> ValidateSharedLinkAsync(string token)
    {
        var link = await _context.SharedLinks
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Token == token && !l.IsUsed && l.ExpiresAt > DateTime.UtcNow);
        if (link == null)
            return new ValidateShareResponseDto { IsValid = false };

        return new ValidateShareResponseDto
        {
            IsValid = true,
            UserId = link.UserId,
            UserName = link.User.DisplayName ?? link.User.Email,
            LibraryAccess = link.GrantsLibraryAccess,
            WishlistAccess = link.GrantsWishlistAccess
        };
    }

    public async Task<IEnumerable<UserSearchResultDto>> SearchUsersAsync(string query, string currentUserId)
    {
        if (string.IsNullOrWhiteSpace(query)) 
            return Enumerable.Empty<UserSearchResultDto>();

        var users = await _context.Users
            .Where(u => u.Id != currentUserId && 
                ((u.DisplayName != null && u.DisplayName.Contains(query)) || u.Email.Contains(query)))
            .Take(10)
            .Select(u => new UserSearchResultDto
            {
                Id = u.Id,
                DisplayName = u.DisplayName ?? u.Email,
                Email = u.Email,
                IsLibraryPublic = u.IsLibraryPublic,
                IsWishlistPublic = u.IsWishlistPublic,
                Avatar = u.Avatar
            })
            .ToListAsync();
        return users;
    }

    public async Task SubscribeAsync(string subscriberId, string targetUserId, string? listType = null)
    {
        if (subscriberId == targetUserId)
            throw new InvalidOperationException("Cannot subscribe to yourself");

        var existing = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.SubscriberUserId == subscriberId && s.TargetUserId == targetUserId && s.ListType == listType);
        if (existing != null) return;

        var subscription = new Subscription
        {
            SubscriberUserId = subscriberId,
            TargetUserId = targetUserId,
            ListType = listType,
            CreatedAt = DateTime.UtcNow
        };
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task UnsubscribeAsync(string subscriberId, string targetUserId, string? listType = null)
    {
        var query = _context.Subscriptions
            .Where(s => s.SubscriberUserId == subscriberId && s.TargetUserId == targetUserId);
        if (listType != null)
            query = query.Where(s => s.ListType == listType);
        var subs = await query.ToListAsync();
        _context.Subscriptions.RemoveRange(subs);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CanAccessListAsync(string currentUserId, string ownerUserId, string listType, string? sharedToken = null)
    {
        _logger.LogInformation("=== Access Check ===");
        _logger.LogInformation("CurrentUser: {0}, Owner: {1}, ListType: {2}, SharedToken: {3}",
            currentUserId, ownerUserId, listType, sharedToken ?? "null");

        // 1. Сам владелец
        if (currentUserId == ownerUserId)
        {
            _logger.LogInformation("Access GRANTED: owner themselves");
            return true;
        }

        // 2. Одноразовая ссылка
        if (!string.IsNullOrEmpty(sharedToken))
        {
            var link = await _context.SharedLinks
                .FirstOrDefaultAsync(l => l.Token == sharedToken && !l.IsUsed && l.ExpiresAt > DateTime.UtcNow);
            if (link != null && link.UserId == ownerUserId)
            {
                bool hasAccess = listType == "library" ? link.GrantsLibraryAccess : link.GrantsWishlistAccess;
                if (hasAccess)
                {
                    link.IsUsed = true;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Access GRANTED via one-time link (will be marked used)");
                    return true;
                }
            }
        }

        // 3. Прямая подписка на конкретный список
        var directSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.SubscriberUserId == currentUserId && s.TargetUserId == ownerUserId && s.ListType == listType);
        if (directSubscription != null)
        {
            _logger.LogInformation("Access GRANTED via direct subscription to {0}", listType);
            return true;
        }

        // 4. Подписка на пользователя (общая) – даёт доступ только к публичным спискам
        var userSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.SubscriberUserId == currentUserId && s.TargetUserId == ownerUserId && s.ListType == null);
        if (userSubscription != null)
        {
            var user = await _context.Users.FindAsync(ownerUserId);
            if (user != null)
            {
                bool isPublic = listType == "library" ? user.IsLibraryPublic : user.IsWishlistPublic;
                if (isPublic)
                {
                    _logger.LogInformation("Access GRANTED via general subscription to public {0}", listType);
                    return true;
                }
            }
        }

        _logger.LogInformation("Access DENIED");
        return false;
    }

    public async Task<IEnumerable<SubscriptionDto>> GetSubscriptionsAsync(string userId)
    {
      // Загружаем все подписки пользователя вместе с целевыми пользователями
      var allSubs = await _context.Subscriptions
          .Include(s => s.TargetUser)
          .Where(s => s.SubscriberUserId == userId)
          .ToListAsync();

      // Группируем по TargetUserId
      var grouped = allSubs
          .GroupBy(s => s.TargetUserId)
          .Select(g => {
              var targetUser = g.First().TargetUser;
              bool hasLibrary = false;
              bool hasWishlist = false;

              foreach (var sub in g)
              {
                  if (sub.ListType == null)
                  {
                      // Общая подписка – даёт доступ к публичным спискам
                      hasLibrary = hasLibrary || targetUser.IsLibraryPublic;
                      hasWishlist = hasWishlist || targetUser.IsWishlistPublic;
                  }
                  else if (sub.ListType == "library")
                  {
                      hasLibrary = true;
                  }
                  else if (sub.ListType == "wishlist")
                  {
                      hasWishlist = true;
                  }
              }

              return new SubscriptionDto
              {
                  Id = g.First().Id,          // любой Id, т.к. объединяем
                  UserId = targetUser.Id,
                  DisplayName = targetUser.DisplayName ?? targetUser.Email,
                  Avatar = targetUser.Avatar,
                  HasLibraryAccess = hasLibrary,
                  HasWishlistAccess = hasWishlist
              };
          });

      return grouped;
    }
}