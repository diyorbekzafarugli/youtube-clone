using FluentValidation;
using Microsoft.Extensions.Localization;
using YouTubeClone.Application.Resources;

namespace YouTubeClone.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator(IStringLocalizer<AuthResource> localizer)
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage(localizer["RefreshTokenRequired"]);
    }
}