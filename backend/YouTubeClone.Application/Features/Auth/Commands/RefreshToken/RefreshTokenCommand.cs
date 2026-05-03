using MediatR;
using YouTubeClone.Application.Features.Auth.DTOs;

namespace YouTubeClone.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; init; } = string.Empty;
}