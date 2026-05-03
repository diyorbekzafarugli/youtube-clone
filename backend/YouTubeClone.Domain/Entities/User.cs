using YouTubeClone.Domain.Enums;

namespace YouTubeClone.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string? ChannelDescription { get; set; }
    public UserRole Role { get; set; } = UserRole.User;

    // Navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Video> Videos { get; set; } = new List<Video>();
    public ICollection<Subscription> Subscribers { get; set; } = new List<Subscription>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}