using MediatR;

namespace YouTubeClone.Application.Features.Comments.Commands.DeleteComment;

public record DeleteCommentCommand : IRequest<bool>
{
    public Guid CommentId { get; init; }
    public Guid UserId { get; init; }
}