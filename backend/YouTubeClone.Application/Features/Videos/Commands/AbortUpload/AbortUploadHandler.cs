using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Videos.Commands.AbortUpload;

public class AbortUploadHandler : IRequestHandler<AbortUploadCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IS3Service _s3Service;
    private readonly ILogger<AbortUploadHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public AbortUploadHandler(
        IUnitOfWork unitOfWork,
        IS3Service s3Service,
        ILogger<AbortUploadHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _s3Service = s3Service;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<bool> Handle(
        AbortUploadCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Aborting upload for video: {VideoId}", request.VideoId);

        var video = await _unitOfWork.Videos
            .GetByIdAsync(request.VideoId, cancellationToken);

        if (video is null)
            throw new NotFoundException(nameof(video), request.VideoId);

        // R2 dan abort qilish
        if (!string.IsNullOrEmpty(request.UploadId))
        {
            await _s3Service.AbortMultipartUploadAsync(
                request.Key,
                request.UploadId,
                isVideo: true,
                cancellationToken);
        }

        // DB dan o'chirish
        await _unitOfWork.Videos.DeleteAsync(video.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Upload aborted for video: {VideoId}", request.VideoId);

        return true;
    }
}