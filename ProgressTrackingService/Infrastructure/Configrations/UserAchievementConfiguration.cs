using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entity;

namespace ProgressTrackingService.Infrastructure.Configrations
{
    public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
    {
        public void Configure(EntityTypeBuilder<UserAchievement> builder)
        {
            builder.ToTable("UserAchievements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.AchievementId).IsRequired();

            builder.Property(x => x.EarnedAt).IsRequired();

            //builder.Property(x => x.CreatedAt).IsRequired();
            //builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}
