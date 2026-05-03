using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Domain.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
}
