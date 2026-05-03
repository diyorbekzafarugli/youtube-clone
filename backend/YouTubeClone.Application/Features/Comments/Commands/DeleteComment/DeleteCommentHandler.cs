using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Comments.Commands.DeleteComment;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCommentHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public DeleteCommentHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteCommentHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<bool> Handle(
        DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting comment: {CommentId}", request.CommentId);

        var comment = await _unitOfWork.Comments
            .GetByIdAsync(request.CommentId, cancellationToken);

        if (comment is null)
            throw new NotFoundException(nameof(comment), request.CommentId);

        // Faqat o'z commentini o'chira oladi
        if (comment.UserId != request.UserId)
            throw new UnauthorizedException(_localizer["UnauthorizedCommentDelete"]);

        await _unitOfWork.Comments.DeleteAsync(comment.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Comment deleted: {CommentId}", request.CommentId);

        return true;
    }
}