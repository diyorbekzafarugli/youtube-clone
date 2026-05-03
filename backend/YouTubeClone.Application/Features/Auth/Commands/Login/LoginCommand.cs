using MediatR;
using YouTubeClone.Application.Features.Auth.DTOs;

namespace YouTubeClone.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}