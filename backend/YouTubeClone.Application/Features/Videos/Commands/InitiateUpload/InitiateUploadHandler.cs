using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Features.Videos.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Enums;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Videos.Commands.InitiateUpload;

public class InitiateUploadHandler : IRequestHandler<InitiateUploadCommand, InitiateUploadDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IS3Service _s3Service;
    private readonly ILogger<InitiateUploadHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    // Har bir part 10MB
    private const long PartSize = 10 * 1024 * 1024;

    public InitiateUploadHandler(
        IUnitOfWork unitOfWork,
        IS3Service s3Service,
        ILogger<InitiateUploadHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _s3Service = s3Service;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<InitiateUploadDto> Handle(
        InitiateUploadCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initiating upload for user: {UserId}", request.UserId);

        // Multipart yoki Simple upload?
        var isMultipart = request.FileSize > PartSize;

        string uploadId;
        string key;

        if (isMultipart)
        {
            // Katta fayl — Multipart upload
            var result = await _s3Service.InitiateMultipartUploadAsync(
                request.FileName,
                request.ContentType,
                isVideo: true,
                cancellationToken);

            var parts = result.Split('|');
            uploadId = parts[0];
            key = parts[1];
        }
        else
        {
            // Kichik fayl — Simple presigned URL
            var result = await _s3Service.GeneratePresignedUploadUrlAsync(
                request.FileName,
                request.ContentType,
                isVideo: true,
                cancellationToken);

            var parts = result.Split('|');
            uploadId = string.Empty;
            key = parts[1];
        }

        // Video DB ga saqlash (Processing holatida)
        var video = new Video
        {
            Title = request.Title,
            Description = request.Description,
            VideoUrl = key, // hozircha key, keyin to'liq URL bo'ladi
            Duration = request.Duration,
            Status = VideoStatus.Processing,
            UserId = request.UserId
        };

        await _unitOfWork.Videos.AddAsync(video, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Video created in DB: {VideoId}", video.Id);

        // Part URL larni yaratish
        var partUrls = new List<PartUploadDto>();

        if (isMultipart)
        {
            var totalParts = (int)Math.Ceiling((double)request.FileSize / PartSize);

            for (int i = 1; i <= totalParts; i++)
            {
                var partUrl = await _s3Service.GeneratePresignedPartUrlAsync(
                    key, uploadId, i, isVideo: true, cancellationToken);

                partUrls.Add(new PartUploadDto
                {
                    PartNumber = i,
                    PresignedUrl = partUrl
                });
            }
        }

        return new InitiateUploadDto
        {
            VideoId = video.Id,
            UploadId = uploadId,
            Key = key,
            Parts = partUrls
        };
    }
}