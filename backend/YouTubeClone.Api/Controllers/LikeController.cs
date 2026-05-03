using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTubeClone.Application.Features.Likes.Commands.ToggleLike;
using YouTubeClone.Application.Features.Likes.Queries.GetVideoLikes;

namespace YouTubeClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LikeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LikeController> _logger;

    public LikeController(IMediator mediator, ILogger<LikeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpPost("toggle")]
    [Authorize]
    public async Task<IActionResult> ToggleLike(
        [FromBody] ToggleLikeCommand command,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(
            command with { UserId = userId },
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{videoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVideoLikes(
        Guid videoId,
        CancellationToken cancellationToken)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? GetUserId()
            : (Guid?)null;

        var result = await _mediator.Send(
            new GetVideoLikesQuery
            {
                VideoId = videoId,
                UserId = userId
            },
            cancellationToken);

        return Ok(result);
    }
}