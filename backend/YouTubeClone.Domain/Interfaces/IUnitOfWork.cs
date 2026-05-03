using YouTubeClone.Domain.Interfaces.Repositories;

namespace YouTubeClone.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IVideoRepository Videos { get; }
    ICommentRepository Comments { get; }
    ILikeRepository Likes { get; }
    ISubscriptionRepository Subscriptions { get; }
    IVideoViewRepository VideoViews { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}