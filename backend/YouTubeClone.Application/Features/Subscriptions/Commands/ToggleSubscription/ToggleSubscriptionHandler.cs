using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using YouTubeClone.Application.Common.Exceptions;
using YouTubeClone.Application.Features.Subscriptions.DTOs;
using YouTubeClone.Application.Resources;
using YouTubeClone.Domain.Entities;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Application.Features.Subscriptions.Commands.ToggleSubscription;

public class ToggleSubscriptionHandler : IRequestHandler<ToggleSubscriptionCommand, SubscriptionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ToggleSubscriptionHandler> _logger;
    private readonly IStringLocalizer<AuthResource> _localizer;

    public ToggleSubscriptionHandler(
        IUnitOfWork unitOfWork,
        ILogger<ToggleSubscriptionHandler> logger,
        IStringLocalizer<AuthResource> localizer)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<SubscriptionDto> Handle(
        ToggleSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Toggle subscription. ChannelId: {ChannelId}", request.ChannelId);

        if (request.SubscriberId == request.ChannelId)
            throw new ValidationException(
                new Dictionary<string, string[]>
                {
                    { "ChannelId", new[] { _localizer["CannotSubscribeToYourself"].Value } }
                });

        var channel = await _unitOfWork.Users
            .GetByIdAsync(request.ChannelId, cancellationToken);

        if (channel is null)
            throw new NotFoundException(nameof(channel), request.ChannelId);

        var existing = await _unitOfWork.Subscriptions
            .GetBySubscriberAndChannelAsync(
                request.SubscriberId,
                request.ChannelId,
                cancellationToken);

        bool isSubscribed;

        if (existing is not null)
        {
            // Obunani bekor qilish
            await _unitOfWork.Subscriptions.DeleteAsync(existing.Id, cancellationToken);
            isSubscribed = false;
            _logger.LogInformation("Unsubscribed from channel: {ChannelId}", request.ChannelId);
        }
        else
        {
            // Obuna bo'lish
            var subscription = new Subscription
            {
                SubscriberId = request.SubscriberId,
                ChannelId = request.ChannelId
            };

            await _unitOfWork.Subscriptions.AddAsync(subscription, cancellationToken);
            isSubscribed = true;
            _logger.LogInformation("Subscribed to channel: {ChannelId}", request.ChannelId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var subscribers = await _unitOfWork.Subscriptions
            .GetByChannelIdAsync(request.ChannelId, cancellationToken);

        return new SubscriptionDto
        {
            ChannelId = channel.Id,
            ChannelName = channel.ChannelName,
            AvatarUrl = channel.AvatarUrl,
            SubscribersCount = subscribers.Count(),
            IsSubscribed = isSubscribed
        };
    }
}