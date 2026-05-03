namespace YouTubeClone.Application.Features.Subscriptions.DTOs;

public class SubscriptionDto
{
    public Guid ChannelId { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public long SubscribersCount { get; set; }
    public bool IsSubscribed { get; set; }
}