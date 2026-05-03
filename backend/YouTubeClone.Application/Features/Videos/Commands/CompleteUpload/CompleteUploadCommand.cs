using MediatR;
using YouTubeClone.Application.Features.Videos.DTOs;

namespace YouTubeClone.Application.Features.Videos.Commands.CompleteUpload;

public record CompleteUploadCommand : IRequest<VideoDto>
{
    public Guid VideoId { get; init; }
    public string Key { get; init; } = string.Empty;
    public string? UploadId { get; init; }
    public List<PartInfo> Parts { get; init; } = new();
    public string? ThumbnailBase64 { get; init; }
    public string? ThumbnailContentType { get; init; }
}
