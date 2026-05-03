using Microsoft.EntityFrameworkCore;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Enums;
using YouTubeClone.Domain.Interfaces.Repositories;
using YouTubeClone.Infrastructure.Persistence;

namespace YouTubeClone.Infrastructure.Repositories;

public class VideoRepository : BaseRepository<Video>, IVideoRepository
{
    public VideoRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<Video?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(v => v.User)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    public override async Task<IEnumerable<Video>> GetAllAsync(CancellationToken cancellationToken = default)
    => await _dbSet
        .Include(v => v.User)
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Video>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(v => v.User)
            .Where(v => v.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task<(IEnumerable<Video> Videos, int TotalCount)> GetPagedAsync(
        Guid? userId,
        VideoStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Include(v => v.User).AsQueryable();

        if (userId.HasValue)
            query = query.Where(v => v.UserId == userId.Value);

        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var videos = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (videos, totalCount);
    }
}