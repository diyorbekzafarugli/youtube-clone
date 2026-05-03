using MediatR;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Videos.DTOs;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Videos.Queries.GetVideo;

public class GetVideoHandler : IRequestHandler<GetVideoQuery, VideoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetVideoHandler> _logger;

    public GetVideoHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetVideoHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<VideoDto> Handle(
        GetVideoQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting video: {VideoId}", request.VideoId);

        var video = await _unitOfWork.Videos
            .GetByIdAsync(request.VideoId, cancellationToken);

        if (video is null)
            throw new NotFoundException(nameof(video), request.VideoId);

        _logger.LogInformation("Video found: {VideoId}", request.VideoId);

        return new VideoDto
        {
            Id = video.Id,
            Title = video.Title,
            Description = video.Description,
            VideoUrl = video.VideoUrl,
            ThumbnailUrl = video.ThumbnailUrl,
            Duration = video.Duration,
            ViewCount = video.ViewCount,
            Status = video.Status,
            UserId = video.UserId,
            ChannelName = video.User?.ChannelName ?? string.Empty,
            AvatarUrl = video.User?.AvatarUrl,
            CreatedAt = video.CreatedAt
        };
    }
}