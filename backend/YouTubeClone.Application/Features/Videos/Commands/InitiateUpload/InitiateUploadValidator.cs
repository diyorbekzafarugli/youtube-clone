using FluentValidation;
using Microsoft.Extensions.Localization;
using YouTubeClone.Application.Resources;

namespace YouTubeClone.Application.Features.Videos.Commands.InitiateUpload;

public class InitiateUploadValidator : AbstractValidator<InitiateUploadCommand>
{
    private static readonly string[] AllowedContentTypes =
    {
        "video/mp4", "video/webm", "video/avi",
        "video/mkv", "video/mov", "video/wmv"
    };

    public InitiateUploadValidator(IStringLocalizer<AuthResource> localizer)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(localizer["VideoTitleRequired"])
            .MaximumLength(500).WithMessage(localizer["VideoTitleTooLong"]);

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage(localizer["FileNameRequired"]);

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage(localizer["ContentTypeRequired"])
            .Must(ct => AllowedContentTypes.Contains(ct))
            .WithMessage(localizer["InvalidVideoFormat"]);

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage(localizer["FileSizeInvalid"])
            .LessThanOrEqualTo(10L * 1024 * 1024 * 1024) // 10GB
            .WithMessage(localizer["FileSizeTooLarge"]);

        RuleFor(x => x.Duration)
            .GreaterThan(0).WithMessage(localizer["DurationInvalid"]);

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(localizer["UserIdRequired"]);
    }
}