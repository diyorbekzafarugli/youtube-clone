using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Features.Videos.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared;

namespace YouTubeClone.Application.Features.Videos.Queries.GetVideos;

public class GetVideosHandler : IRequestHandler<GetVideosQuery, PagedResult<VideoDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetVideosHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public GetVideosHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetVideosHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<PagedResult<VideoDto>> Handle(
        GetVideosQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting videos. Page: {Page}", request.Page);

        var (videos, totalCount) = await _unitOfWork.Videos.GetPagedAsync(
            request.UserId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = videos.Select(v => new VideoDto
        {
            Id = v.Id,
            Title = v.Title,
            Description = v.Description,
            VideoUrl = v.VideoUrl,
            ThumbnailUrl = v.ThumbnailUrl,
            Duration = v.Duration,
            ViewCount = v.ViewCount,
            Status = v.Status,
            UserId = v.UserId,
            ChannelName = v.User?.ChannelName ?? string.Empty,
            AvatarUrl = v.User?.AvatarUrl,
            CreatedAt = v.CreatedAt
        }).ToList();

        return new PagedResult<VideoDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}