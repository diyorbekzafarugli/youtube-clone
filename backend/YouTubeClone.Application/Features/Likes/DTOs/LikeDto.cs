namespace YouTubeClone.Application.Features.Likes.DTOs;

public class LikeDto
{
    public Guid VideoId { get; set; }
    public long LikesCount { get; set; }
    public long DislikesCount { get; set; }
    public bool? UserLikeStatus { get; set; } // true=like, false=dislike, null=none
}