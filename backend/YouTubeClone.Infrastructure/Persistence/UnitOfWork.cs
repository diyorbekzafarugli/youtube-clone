using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Domain.Interfaces.Repositories;
using YouTubeClone.Infrastructure.Repositories;

namespace YouTubeClone.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IUserRepository Users { get; }
    public IVideoRepository Videos { get; }
    public ICommentRepository Comments { get; }
    public ILikeRepository Likes { get; }
    public ISubscriptionRepository Subscriptions { get; }
    public IVideoViewRepository VideoViews { get; }
    public IRefreshTokenRepository RefreshTokens { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Videos = new VideoRepository(context);
        Comments = new CommentRepository(context);
        Likes = new LikeRepository(context);
        Subscriptions = new SubscriptionRepository(context);
        VideoViews = new VideoViewRepository(context);
        RefreshTokens = new RefreshTokenRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
        => _context.Dispose();
}