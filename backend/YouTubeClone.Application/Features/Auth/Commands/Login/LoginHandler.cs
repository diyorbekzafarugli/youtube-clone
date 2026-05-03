using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Auth.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public LoginHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        ILogger<LoginHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempting: {Email}", request.Email);

        var user = await _unitOfWork.Users
            .GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User not found: {Email}", request.Email);
            throw new UnauthorizedException(_localizer["InvalidCredentials"]);
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Invalid password: {Email}", request.Email);
            throw new UnauthorizedException(_localizer["InvalidCredentials"]);
        }

        _logger.LogInformation("Login successful: {UserId}", user.Id);

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

        await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            ChannelName = user.ChannelName,
            AvatarUrl = user.AvatarUrl,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddDays(1),
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }
}