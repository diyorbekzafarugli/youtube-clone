using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YouTubeClone.Application.Features.Subscriptions.Commands.ToggleSubscription;
using YouTubeClone.Application.Features.Subscriptions.Queries.GetSubscriptions;

namespace YouTubeClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(IMediator mediator, ILogger<SubscriptionController> logger)
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
    public async Task<IActionResult> ToggleSubscription(
        [FromBody] ToggleSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        var subscriberId = GetUserId();
        var result = await _mediator.Send(
            command with { SubscriberId = subscriberId },
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMySubscriptions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(
            new GetSubscriptionsQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            },
            cancellationToken);
        return Ok(result);
    }
}