using MediatR;
using YouTubeClone.Application.Features.Videos.DTOs;

namespace YouTubeClone.Application.Features.Videos.Commands.InitiateUpload;

public record InitiateUploadCommand : IRequest<InitiateUploadDto>
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSize { get; init; } // bytes
    public int Duration { get; init; }
    public Guid UserId { get; init; }
}