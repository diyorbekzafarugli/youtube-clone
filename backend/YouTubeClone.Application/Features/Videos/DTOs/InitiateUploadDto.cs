namespace YouTubeClone.Application.Features.Videos.DTOs;

public class InitiateUploadDto
{
    public Guid VideoId { get; set; }
    public string UploadId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public List<PartUploadDto> Parts { get; set; } = new();
}