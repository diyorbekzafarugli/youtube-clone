namespace YouTubeClone.Domain.Entities;

public class Subscription : BaseEntity
{
    public Guid SubscriberId { get; set; }  // obuna bo'lgan
    public Guid ChannelId { get; set; }     // obuna bo'lingan kanal

    public User Subscriber { get; set; } = null!;
    public User Channel { get; set; } = null!;
}