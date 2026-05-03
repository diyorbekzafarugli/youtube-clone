using FluentValidation;
using Microsoft.Extensions.Localization;
using YouTubeClone.Application.Resources;

namespace YouTubeClone.Application.Features.Comments.Commands.AddComment;

public class AddCommentValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentValidator(IStringLocalizer<AuthResource> localizer)
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(localizer["CommentRequired"])
            .MaximumLength(2000).WithMessage(localizer["CommentTooLong"]);

        RuleFor(x => x.VideoId)
            .NotEmpty().WithMessage(localizer["VideoIdRequired"]);
    }
}