namespace YouTubeClone.Application.Features.Videos.Commands.CompleteUpload;

public record PartInfo
{
    public int PartNumber { get; init; }
    public string ETag { get; init; } = string.Empty;
}