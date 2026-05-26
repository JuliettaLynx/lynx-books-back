using LynxBooks.Backend.DTOs.Community;

namespace LynxBooks.Backend.Services.Interfaces;

public interface ICommunityService
{
    Task<PrivacySettingsDto> GetPrivacySettingsAsync(string userId);
    Task UpdatePrivacySettingsAsync(string userId, PrivacySettingsDto settings);
    Task<SharedLinkResponseDto> GenerateSharedLinkAsync(string userId, bool grantLibrary, bool grantWishlist);
    Task<ValidateShareResponseDto> ValidateSharedLinkAsync(string token);
    Task<IEnumerable<UserSearchResultDto>> SearchUsersAsync(string query, string currentUserId);
    Task SubscribeAsync(string subscriberId, string targetUserId, string? listType = null);
    Task UnsubscribeAsync(string subscriberId, string targetUserId, string? listType = null);
    Task<IEnumerable<SubscriptionDto>> GetSubscriptionsAsync(string userId);
    Task<bool> CanAccessListAsync(string currentUserId, string ownerUserId, string listType, string? sharedToken = null);
}