using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Domain.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Subscription?> GetBySubscriberAndChannelAsync(Guid subscriberId, 
        Guid channelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetBySubscriberIdAsync(Guid subscriberId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Subscription>> GetByChannelIdAsync(Guid channelId, CancellationToken cancellationToken = default);
    Task AddAsync(Subscription subscription, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}