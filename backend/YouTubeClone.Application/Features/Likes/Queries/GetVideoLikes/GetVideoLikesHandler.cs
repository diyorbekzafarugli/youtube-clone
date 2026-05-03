using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Likes.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Likes.Queries.GetVideoLikes;

public class GetVideoLikesHandler : IRequestHandler<GetVideoLikesQuery, LikeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetVideoLikesHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public GetVideoLikesHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetVideoLikesHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<LikeDto> Handle(
        GetVideoLikesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting likes. VideoId: {VideoId}", request.VideoId);

        var video = await _unitOfWork.Videos
            .GetByIdAsync(request.VideoId, cancellationToken);

        if (video is null)
            throw new NotFoundException(nameof(video), request.VideoId);

        var likes = await _unitOfWork.Likes
            .GetByVideoIdAsync(request.VideoId, cancellationToken);

        Domain.Entities.Like? userLike = null;

        if (request.UserId.HasValue)
            userLike = await _unitOfWork.Likes
                .GetByUserAndVideoAsync(request.UserId.Value, request.VideoId, cancellationToken);

        return new LikeDto
        {
            VideoId = request.VideoId,
            LikesCount = likes.Count(l => l.IsLike),
            DislikesCount = likes.Count(l => !l.IsLike),
            UserLikeStatus = userLike?.IsLike
        };
    }
}