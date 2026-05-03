using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Domain.Interfaces.Repositories;

public interface ILikeRepository
{
    Task<Like?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Like?> GetByUserAndVideoAsync(Guid userId, Guid videoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Like>> GetByVideoIdAsync(Guid videoId, CancellationToken cancellationToken = default);
    Task AddAsync(Like like, CancellationToken cancellationToken = default);
    Task UpdateAsync(Like like, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}