using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Likes.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Likes.Commands.ToggleLike;

public class ToggleLikeHandler : IRequestHandler<ToggleLikeCommand, LikeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ToggleLikeHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public ToggleLikeHandler(
        IUnitOfWork unitOfWork,
        ILogger<ToggleLikeHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<LikeDto> Handle(
        ToggleLikeCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Toggle like. VideoId: {VideoId} UserId: {UserId}",
            request.VideoId, request.UserId);

        var video = await _unitOfWork.Videos
            .GetByIdAsync(request.VideoId, cancellationToken);

        if (video is null)
            throw new NotFoundException(nameof(video), request.VideoId);

        var existingLike = await _unitOfWork.Likes
            .GetByUserAndVideoAsync(request.UserId, request.VideoId, cancellationToken);

        if (existingLike is not null)
        {
            if (existingLike.IsLike == request.IsLike)
            {
                // Xuddi shunday like/dislike — o'chiramiz
                await _unitOfWork.Likes.DeleteAsync(existingLike.Id, cancellationToken);
                _logger.LogInformation("Like removed. VideoId: {VideoId}", request.VideoId);
            }
            else
            {
                // Like → Dislike yoki aksincha
                existingLike.IsLike = request.IsLike;
                existingLike.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Likes.UpdateAsync(existingLike, cancellationToken);
                _logger.LogInformation("Like updated. VideoId: {VideoId}", request.VideoId);
            }
        }
        else
        {
            // Yangi like/dislike
            var like = new Like
            {
                VideoId = request.VideoId,
                UserId = request.UserId,
                IsLike = request.IsLike
            };

            await _unitOfWork.Likes.AddAsync(like, cancellationToken);
            _logger.LogInformation("Like added. VideoId: {VideoId}", request.VideoId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Statistika
        var likes = await _unitOfWork.Likes
            .GetByVideoIdAsync(request.VideoId, cancellationToken);

        var userLike = await _unitOfWork.Likes
            .GetByUserAndVideoAsync(request.UserId, request.VideoId, cancellationToken);

        return new LikeDto
        {
            VideoId = request.VideoId,
            LikesCount = likes.Count(l => l.IsLike),
            DislikesCount = likes.Count(l => !l.IsLike),
            UserLikeStatus = userLike?.IsLike
        };
    }
}