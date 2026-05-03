using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(s => s.Id);

        // Bir user bir kanalga faqat bir marta obuna bo'la oladi
        builder.HasIndex(s => new { s.SubscriberId, s.ChannelId }).IsUnique();

        // Subscriber
        builder.HasOne(s => s.Subscriber)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.SubscriberId)
            .OnDelete(DeleteBehavior.Restrict); // Cascade o'rniga Restrict

        // Channel
        builder.HasOne(s => s.Channel)
            .WithMany(u => u.Subscribers)
            .HasForeignKey(s => s.ChannelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}