using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Enums;

namespace YouTubeClone.Domain.Interfaces.Repositories;

public interface IVideoRepository
{
    Task<Video?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Video>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Video>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    // Pagination + Filter
    Task<(IEnumerable<Video> Videos, int TotalCount)> GetPagedAsync(
        Guid? userId,
        VideoStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Video video, CancellationToken cancellationToken = default);
    Task UpdateAsync(Video video, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}