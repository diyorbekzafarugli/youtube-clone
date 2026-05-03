namespace YouTubeClone.Application.Features.Videos.DTOs;

public class PartUploadDto
{
    public int PartNumber { get; set; }
    public string PresignedUrl { get; set; } = string.Empty;
}