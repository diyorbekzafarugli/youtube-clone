using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Comments.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared;

namespace YouTubeClone.Application.Features.Comments.Queries.GetVideoComments;

public class GetVideoCommentsHandler : IRequestHandler<GetVideoCommentsQuery, PagedResult<CommentDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetVideoCommentsHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public GetVideoCommentsHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetVideoCommentsHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<PagedResult<CommentDto>> Handle(
        GetVideoCommentsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting comments. VideoId: {VideoId}", request.VideoId);

        var video = await _unitOfWork.Videos
            .GetByIdAsync(request.VideoId, cancellationToken);

        if (video is null)
            throw new NotFoundException(nameof(video), request.VideoId);

        var comments = await _unitOfWork.Comments
            .GetByVideoIdAsync(request.VideoId, cancellationToken);

        var items = comments
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                UserId = c.UserId,
                Username = c.User?.Username ?? string.Empty,
                AvatarUrl = c.User?.AvatarUrl,
                VideoId = c.VideoId,
                CreatedAt = c.CreatedAt
            })
            .ToList();

        return new PagedResult<CommentDto>
        {
            Items = items,
            TotalCount = comments.Count(),
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}