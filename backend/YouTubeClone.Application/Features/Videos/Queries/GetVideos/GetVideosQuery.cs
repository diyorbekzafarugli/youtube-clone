using MediatR;
using YouTubeClone.Application.Features.Videos.DTOs;
using YouTubeClone.Domain.Enums;
using YouTubeClone.Shared;

namespace YouTubeClone.Application.Features.Videos.Queries.GetVideos;

public record GetVideosQuery : IRequest<PagedResult<VideoDto>>
{
    public Guid? UserId { get; init; }
    public VideoStatus? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}