namespace YouTubeClone.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;

    public Guid UserId { get; set; }
    public Guid VideoId { get; set; }

    public User User { get; set; } = null!;
    public Video Video { get; set; } = null!;
}