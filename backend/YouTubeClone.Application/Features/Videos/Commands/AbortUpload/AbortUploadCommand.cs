using MediatR;

namespace YouTubeClone.Application.Features.Videos.Commands.AbortUpload;

public record AbortUploadCommand : IRequest<bool>
{
    public Guid VideoId { get; init; }
    public string Key { get; init; } = string.Empty;
    public string UploadId { get; init; } = string.Empty;
}