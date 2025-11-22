using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entity;

namespace ProgressTrackingService.Infrastructure.Configrations
{
    public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.ToTable("Achievements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Description)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(x => x.IconUrl)
                   .HasMaxLength(300);

            //builder.Property(x => x.CreatedAt).IsRequired();
            //builder.Property(x => x.UpdatedAt).IsRequired();


            builder.HasMany(a => a.UserAchievements)
                                       .WithOne(ua => ua.Achievement)
                                       .HasForeignKey(ua => ua.AchievementId)
                                       .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
