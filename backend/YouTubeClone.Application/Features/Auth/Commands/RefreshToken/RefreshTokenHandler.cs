using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Auth.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RefreshTokenHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public RefreshTokenHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        ILogger<RefreshTokenHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<AuthResponseDto> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("RefreshToken attempting");

        // Token DB da bormi?
        var refreshToken = await _unitOfWork.RefreshTokens
            .GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            _logger.LogWarning("RefreshToken not found");
            throw new UnauthorizedException(_localizer["InvalidRefreshToken"]);
        }

        // Token aktivmi?
        if (!refreshToken.IsActive)
        {
            _logger.LogWarning("RefreshToken is not active: {UserId}", refreshToken.UserId);
            throw new UnauthorizedException(_localizer["RefreshTokenExpired"]);
        }

        // Eski tokenni revoke qilamiz
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.RefreshTokens.UpdateAsync(refreshToken, cancellationToken);

        // Yangi tokenlar yaratamiz
        var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.User);
        var newRefreshToken = _jwtService.GenerateRefreshToken(refreshToken.UserId);

        // Yangi refresh tokenni DB ga saqlash
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tokens refreshed for user: {UserId}", refreshToken.UserId);

        return new AuthResponseDto
        {
            UserId = refreshToken.User.Id,
            Username = refreshToken.User.Username,
            Email = refreshToken.User.Email,
            ChannelName = refreshToken.User.ChannelName,
            AvatarUrl = refreshToken.User.AvatarUrl,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddDays(1),
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
        };
    }
}
