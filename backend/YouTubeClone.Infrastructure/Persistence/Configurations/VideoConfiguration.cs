using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Infrastructure.Persistence.Configurations;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.ToTable("Videos");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.Description)
            .HasMaxLength(5000);

        builder.Property(v => v.VideoUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(v => v.ThumbnailUrl)
            .HasMaxLength(1000);

        builder.Property(v => v.Duration)
            .IsRequired();

        builder.Property(v => v.ViewCount)
            .HasDefaultValue(0);

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<string>(); // Enum → String saqlanadi

        // Relationships
        builder.HasMany(v => v.Comments)
            .WithOne(c => c.Video)
            .HasForeignKey(c => c.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Likes)
            .WithOne(l => l.Video)
            .HasForeignKey(l => l.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Views)
            .WithOne(vv => vv.Video)
            .HasForeignKey(vv => vv.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
