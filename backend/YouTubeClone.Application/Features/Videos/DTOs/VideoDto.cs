using YouTubeClone.Domain.Enums;

namespace YouTubeClone.Application.Features.Videos.DTOs;

public class VideoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int Duration { get; set; }
    public long ViewCount { get; set; }
    public VideoStatus Status { get; set; }
    public Guid UserId { get; set; }
    public string ChannelName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}