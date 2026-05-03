using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Auth.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RegisterHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public RegisterHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        ILogger<RegisterHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<AuthResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Register attempting: {Email}", request.Email);

        var existingUser = await _unitOfWork.Users
            .GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser is not null)
        {
            _logger.LogWarning("Email is already registered: {Email}", request.Email);
            throw new ValidationException(
                new Dictionary<string, string[]>
                {
                    { "Email", new[] { _localizer["EmailAlreadyExists"].Value } }
                });
        }

        var existingUsername = await _unitOfWork.Users
            .GetByUsernameAsync(request.Username, cancellationToken);

        if (existingUsername is not null)
        {
            _logger.LogWarning("Username is already taken: {Username}", request.Username);
            throw new ValidationException(
                new Dictionary<string, string[]>
                {
                    { "Username", new[] { _localizer["UsernameAlreadyTaken"].Value } }
                });
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHash,
            ChannelName = request.ChannelName
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User created: {UserId}", user.Id);

        // AccessToken va RefreshToken yaratish
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

        // RefreshToken ni DB ga saqlash
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tokens generated for user: {UserId}", user.Id);

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