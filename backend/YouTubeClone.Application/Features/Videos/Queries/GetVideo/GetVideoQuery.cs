using MediatR;
using YouTubeClone.Application.Features.Videos.DTOs;

namespace YouTubeClone.Application.Features.Videos.Queries.GetVideo;

public record GetVideoQuery : IRequest<VideoDto>
{
    public Guid VideoId { get; init; }
}