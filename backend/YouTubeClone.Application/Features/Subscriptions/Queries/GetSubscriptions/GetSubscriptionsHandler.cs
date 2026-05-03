using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Features.Subscriptions.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared;

namespace YouTubeClone.Application.Features.Subscriptions.Queries.GetSubscriptions;

public class GetSubscriptionsHandler : IRequestHandler<GetSubscriptionsQuery, PagedResult<SubscriptionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetSubscriptionsHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public GetSubscriptionsHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetSubscriptionsHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<PagedResult<SubscriptionDto>> Handle(
        GetSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting subscriptions. UserId: {UserId}", request.UserId);

        var subscriptions = await _unitOfWork.Subscriptions
            .GetBySubscriberIdAsync(request.UserId, cancellationToken);

        var items = subscriptions
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SubscriptionDto
            {
                ChannelId = s.ChannelId,
                ChannelName = s.Channel?.ChannelName ?? string.Empty,
                AvatarUrl = s.Channel?.AvatarUrl,
                SubscribersCount = 0,
                IsSubscribed = true
            })
            .ToList();

        return new PagedResult<SubscriptionDto>
        {
            Items = items,
            TotalCount = subscriptions.Count(),
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}