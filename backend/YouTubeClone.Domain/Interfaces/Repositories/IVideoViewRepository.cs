using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Domain.Interfaces.Repositories;

public interface IVideoViewRepository
{
    Task<IEnumerable<VideoView>> GetByVideoIdAsync(Guid videoId, CancellationToken cancellationToken = default);
    Task<long> GetViewCountAsync(Guid videoId, CancellationToken cancellationToken = default);
    Task AddAsync(VideoView videoView, CancellationToken cancellationToken = default);
}