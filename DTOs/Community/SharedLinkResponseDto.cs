namespace LynxBooks.Backend.DTOs.Community;

public class SharedLinkResponseDto
{
    public string Link { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}