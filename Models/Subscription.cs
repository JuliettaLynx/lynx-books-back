using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LynxBooks.Backend.Models;

public class Subscription
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? ListType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string SubscriberUserId { get; set; } = string.Empty;
    [ForeignKey(nameof(SubscriberUserId))]
    public User Subscriber { get; set; } = null!;
    
    public string TargetUserId { get; set; } = string.Empty;
    [ForeignKey(nameof(TargetUserId))]
    public User TargetUser { get; set; } = null!;
}