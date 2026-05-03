using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Infrastructure.Persistence.Configurations;

public class LikeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.ToTable("Likes");

        builder.HasKey(l => l.Id);

        // Bir user bir videoga faqat bir marta like bosa oladi
        builder.HasIndex(l => new { l.UserId, l.VideoId }).IsUnique();

        builder.Property(l => l.IsLike)
            .IsRequired();
    }
}
