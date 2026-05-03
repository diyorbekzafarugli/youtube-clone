using Microsoft.EntityFrameworkCore;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces.Repositories;
using YouTubeClone.Infrastructure.Persistence;

namespace YouTubeClone.Infrastructure.Repositories;

public class VideoViewRepository : IVideoViewRepository
{
    private readonly AppDbContext _context;

    public VideoViewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<VideoView>> GetByVideoIdAsync(
        Guid videoId, CancellationToken cancellationToken = default)
        => await _context.VideoViews
            .Where(vv => vv.VideoId == videoId)
            .ToListAsync(cancellationToken);

    public async Task<long> GetViewCountAsync(Guid videoId,
        CancellationToken cancellationToken = default)
        => await _context.VideoViews
            .CountAsync(vv => vv.VideoId == videoId, cancellationToken);

    public async Task AddAsync(VideoView videoView,
        CancellationToken cancellationToken = default)
        => await _context.VideoViews
            .AddAsync(videoView, cancellationToken);
}