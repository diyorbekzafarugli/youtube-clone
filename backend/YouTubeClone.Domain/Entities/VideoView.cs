namespace YouTubeClone.Domain.Entities;

public class VideoView : BaseEntity
{
    public Guid VideoId { get; set; }
    public Guid? UserId { get; set; } // null = anonymous user

    public Video Video { get; set; } = null!;
    public User? User { get; set; }
}