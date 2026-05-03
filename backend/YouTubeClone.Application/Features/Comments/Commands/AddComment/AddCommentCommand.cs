using MediatR;
using YouTubeClone.Application.Features.Comments.DTOs;

namespace YouTubeClone.Application.Features.Comments.Commands.AddComment;

public record AddCommentCommand : IRequest<CommentDto>
{
    public string Content { get; init; } = string.Empty;
    public Guid VideoId { get; init; }
    public Guid UserId { get; init; }
}