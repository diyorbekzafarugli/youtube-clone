using MediatR;
using YouTubeClone.Application.Features.Likes.DTOs;

namespace YouTubeClone.Application.Features.Likes.Commands.ToggleLike;

public record ToggleLikeCommand : IRequest<LikeDto>
{
    public Guid VideoId { get; init; }
    public Guid UserId { get; init; }
    public bool IsLike { get; init; }
}