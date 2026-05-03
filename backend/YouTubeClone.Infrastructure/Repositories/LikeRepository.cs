using Microsoft.EntityFrameworkCore;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces.Repositories;
using YouTubeClone.Infrastructure.Persistence;

namespace YouTubeClone.Infrastructure.Repositories;

public class LikeRepository : BaseRepository<Like>, ILikeRepository
{
    public LikeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Like?> GetByUserAndVideoAsync(Guid userId, Guid videoId,
        CancellationToken cancellationToken = default)
        => await _dbSet
        .FirstOrDefaultAsync(l => l.UserId == userId && l.VideoId == videoId, cancellationToken);

    public async Task<IEnumerable<Like>> GetByVideoIdAsync(Guid videoId,
        CancellationToken cancellationToken = default)
        => await _dbSet.Where(l => l.VideoId == videoId)
                .ToListAsync(cancellationToken);

    public Task UpdateAsync(Like like, CancellationToken cancellationToken = default)
    {
        _context.Likes.Update(like);
        return Task.CompletedTask;
    }
}