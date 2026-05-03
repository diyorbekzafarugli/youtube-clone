using FluentValidation;
using Microsoft.Extensions.Localization;
using YouTubeClone.Application.Common.Validators;
using YouTubeClone.Application.Resources;

namespace YouTubeClone.Application.Features.Auth.Commands.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator(IStringLocalizer<AuthResource> localizer)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(localizer["UsernameRequired"])
            .MinimumLength(3).WithMessage(localizer["UsernameTooShort"])
            .MaximumLength(50).WithMessage(localizer["UsernameTooLong"]);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer["EmailRequired"])
            .EmailAddress().WithMessage(localizer["EmailInvalid"]);

        // Password validatsiyasi
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer["PasswordRequired"])
            .MinimumLength(8).WithMessage(localizer["PasswordTooShort"])
            .Must(PasswordValidator.HasUppercase).WithMessage(localizer["PasswordUppercase"])
            .Must(PasswordValidator.HasLowercase).WithMessage(localizer["PasswordLowercase"])
            .Must(PasswordValidator.HasDigit).WithMessage(localizer["PasswordDigit"])
            .Must(PasswordValidator.HasSpecialChar).WithMessage(localizer["PasswordSpecialChar"]);

        RuleFor(x => x.ChannelName)
            .NotEmpty().WithMessage(localizer["ChannelNameRequired"])
            .MaximumLength(100).WithMessage(localizer["ChannelNameTooLong"]);
    }
}