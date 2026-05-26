namespace LynxBooks.Backend.DTOs.Community;

public class ValidateShareResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool LibraryAccess { get; set; }
    public bool WishlistAccess { get; set; }
    public bool IsValid { get; set; }
}