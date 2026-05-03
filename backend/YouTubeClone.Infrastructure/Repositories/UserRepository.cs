using Microsoft.EntityFrameworkCore;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces.Repositories;
using YouTubeClone.Infrastructure.Persistence;

namespace YouTubeClone.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email,
        CancellationToken cancellationToken = default)
        => await _dbSet
        .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByUsernameAsync(string username,
        CancellationToken cancellationToken = default)
        => await _dbSet
        .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
}
