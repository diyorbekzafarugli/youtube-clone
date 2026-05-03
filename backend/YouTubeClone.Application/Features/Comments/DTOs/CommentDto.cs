namespace YouTubeClone.Application.Features.Comments.DTOs;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public Guid VideoId { get; set; }
    public DateTime CreatedAt { get; set; }
}