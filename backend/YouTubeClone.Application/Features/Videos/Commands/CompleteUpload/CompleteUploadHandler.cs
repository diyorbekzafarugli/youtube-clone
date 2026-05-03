using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Videos.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Enums;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Videos.Commands.CompleteUpload;

public class CompleteUploadHandler : IRequestHandler<CompleteUploadCommand, VideoDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IS3Service _s3Service;
    private readonly ILogger<CompleteUploadHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public CompleteUploadHandler(
        IUnitOfWork unitOfWork,
        IS3Service s3Service,
        ILogger<CompleteUploadHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _s3Service = s3Service;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<VideoDto> Handle(
        CompleteUploadCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing upload for video: {VideoId}", request.VideoId);

        var video = await _unitOfWork.Videos
            .GetByIdAsync(request.VideoId, cancellationToken);

        if (video is null)
            throw new NotFoundException(nameof(video), request.VideoId);

        string videoUrl;

        // Multipart yoki Simple?
        if (!string.IsNullOrEmpty(request.UploadId) && request.Parts.Any())
        {
            // Multipart complete
            videoUrl = await _s3Service.CompleteMultipartUploadAsync(
                request.Key,
                request.UploadId,
                request.Parts.Select(p => (p.PartNumber, p.ETag)).ToList(),
                isVideo: true,
                cancellationToken);
        }
        else
        {
            // Simple upload — URL ni yaratamiz
            videoUrl = request.Key;
        }

        // Thumbnail yuklash
        if (!string.IsNullOrEmpty(request.ThumbnailBase64))
        {
            var thumbnailBytes = Convert.FromBase64String(request.ThumbnailBase64);
            using var thumbnailStream = new MemoryStream(thumbnailBytes);

            var thumbnailUrl = await _s3Service.UploadThumbnailAsync(
                thumbnailStream,
                $"{request.VideoId}.jpg",
                request.ThumbnailContentType ?? "image/jpeg",
                cancellationToken);

            video.ThumbnailUrl = thumbnailUrl;
        }

        // Video ni yangilash
        video.VideoUrl = videoUrl;
        video.Status = VideoStatus.Published;
        video.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Videos.UpdateAsync(video, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Video published: {VideoId}", video.Id);

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
            CreatedAt = video.CreatedAt
        };
    }
}