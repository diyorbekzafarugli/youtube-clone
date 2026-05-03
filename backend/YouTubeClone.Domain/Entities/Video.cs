using YouTubeClone.Domain.Enums;

namespace YouTubeClone.Domain.Entities;

public class Video : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int Duration { get; set; } // sekundda
    public long ViewCount { get; set; } = 0;
    public VideoStatus Status { get; set; } = VideoStatus.Processing;

    // Foreign key
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<VideoView> Views { get; set; } = new List<VideoView>();
}