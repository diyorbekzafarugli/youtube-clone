using MediatR;
using YouTubeClone.Application.Features.Subscriptions.DTOs;

namespace YouTubeClone.Application.Features.Subscriptions.Commands.ToggleSubscription;

public record ToggleSubscriptionCommand : IRequest<SubscriptionDto>
{
    public Guid SubscriberId { get; init; }
    public Guid ChannelId { get; init; }
}