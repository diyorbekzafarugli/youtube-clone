using MediatR;
using YouTubeClone.Application.Features.Likes.DTOs;

namespace YouTubeClone.Application.Features.Likes.Queries.GetVideoLikes;

public record GetVideoLikesQuery : IRequest<LikeDto>
{
    public Guid VideoId { get; init; }
    public Guid? UserId { get; init; }
}