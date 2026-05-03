using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTubeClone.Application.Features.Comments.Commands.AddComment;
using YouTubeClone.Application.Features.Comments.Commands.DeleteComment;
using YouTubeClone.Application.Features.Comments.Queries.GetVideoComments;

namespace YouTubeClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CommentController> _logger;

    public CommentController(IMediator mediator, ILogger<CommentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddComment(
        [FromBody] AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(
            command with { UserId = userId },
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(
        Guid commentId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(
            new DeleteCommentCommand
            {
                CommentId = commentId,
                UserId = userId
            },
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{videoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVideoComments(
        Guid videoId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetVideoCommentsQuery
            {
                VideoId = videoId,
                Page = page,
                PageSize = pageSize
            },
            cancellationToken);
        return Ok(result);
    }
}