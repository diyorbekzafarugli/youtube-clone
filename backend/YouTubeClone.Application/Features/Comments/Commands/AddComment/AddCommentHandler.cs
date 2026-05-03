using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Comments.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Comments.Commands.AddComment;

public class AddCommentHandler : IRequestHandler<AddCommentCommand, CommentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddCommentHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public AddCommentHandler(
        IUnitOfWork unitOfWork,
        ILogger<AddCommentHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<CommentDto> Handle(
        AddCommentCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding comment. VideoId: {VideoId}", request.VideoId);

        var video = await _unitOfWork.Videos
            .GetByIdAsync(request.VideoId, cancellationToken);

        if (video is null)
            throw new NotFoundException(nameof(video), request.VideoId);

        var user = await _unitOfWork.Users
            .GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException(nameof(user), request.UserId);

        var comment = new Comment
        {
            Content = request.Content,
            VideoId = request.VideoId,
            UserId = request.UserId
        };

        await _unitOfWork.Comments.AddAsync(comment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Comment added: {CommentId}", comment.Id);

        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            UserId = comment.UserId,
            Username = user.Username,
            AvatarUrl = user.AvatarUrl,
            VideoId = comment.VideoId,
            CreatedAt = comment.CreatedAt
        };
    }
}