using MediatR;
using YouTubeClone.Application.Features.Subscriptions.DTOs;
using YouTubeClone.Shared;

namespace YouTubeClone.Application.Features.Subscriptions.Queries.GetSubscriptions;

public record GetSubscriptionsQuery : IRequest<PagedResult<SubscriptionDto>>
{
    public Guid UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}