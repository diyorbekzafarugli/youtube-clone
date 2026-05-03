using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTubeClone.Application.Features.Videos.Commands.AbortUpload;
using YouTubeClone.Application.Features.Videos.Commands.CompleteUpload;
using YouTubeClone.Application.Features.Videos.Commands.InitiateUpload;
using YouTubeClone.Application.Features.Videos.Queries.GetVideo;
using YouTubeClone.Application.Features.Videos.Queries.GetVideos;

namespace YouTubeClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VideoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VideoController> _logger;

    public VideoController(IMediator mediator, ILogger<VideoController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException());

    [HttpPost("initiate-upload")]
    public async Task<IActionResult> InitiateUpload(
        [FromBody] InitiateUploadCommand command,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _mediator.Send(
            command with { UserId = userId },
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("complete-upload")]
    public async Task<IActionResult> CompleteUpload(
        [FromBody] CompleteUploadCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("abort-upload")]
    public async Task<IActionResult> AbortUpload(
        [FromBody] AbortUploadCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{videoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVideo(
    Guid videoId,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetVideoQuery { VideoId = videoId },
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetVideos(
    [FromQuery] Guid? userId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetVideosQuery
            {
                UserId = userId,
                Status = Domain.Enums.VideoStatus.Published,
                Page = page,
                PageSize = pageSize
            },
            cancellationToken);

        return Ok(result);
    }
}