namespace YouTubeClone.Domain.Entities;

public class Like : BaseEntity
{
    public bool IsLike { get; set; } // true = like, false = dislike

    public Guid UserId { get; set; }
    public Guid VideoId { get; set; }

    public User User { get; set; } = null!;
    public Video Video { get; set; } = null!;
}