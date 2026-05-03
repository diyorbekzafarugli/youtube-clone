using Microsoft.EntityFrameworkCore;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces.Repositories;
using YouTubeClone.Infrastructure.Persistence;

namespace YouTubeClone.Infrastructure.Repositories;

public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Subscription?> GetBySubscriberAndChannelAsync(Guid subscriberId,
        Guid channelId, CancellationToken cancellationToken = default)
        => await _dbSet
            .FirstOrDefaultAsync(s => s.SubscriberId == subscriberId && s.ChannelId == channelId, cancellationToken);

    public async Task<IEnumerable<Subscription>> GetBySubscriberIdAsync(
        Guid subscriberId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(s => s.SubscriberId == subscriberId)
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<Subscription>> GetByChannelIdAsync(
        Guid channelId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(s => s.ChannelId == channelId)
            .ToListAsync(cancellationToken);
}