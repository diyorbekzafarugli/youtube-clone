using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Infrastructure.Persistence.Configurations;

public class VideoViewConfiguration : IEntityTypeConfiguration<VideoView>
{
    public void Configure(EntityTypeBuilder<VideoView> builder)
    {
        builder.ToTable("VideoViews");

        builder.HasKey(vv => vv.Id);

        builder.HasOne(vv => vv.User)
            .WithMany()
            .HasForeignKey(vv => vv.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
