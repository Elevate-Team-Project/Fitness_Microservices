using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entity;

namespace ProgressTrackingService.Infrastructure.Configrations
{
    public class UserStatisticsConfiguration : IEntityTypeConfiguration<UserStatistics>
    {
        public void Configure(EntityTypeBuilder<UserStatistics> builder)
        {
            builder.ToTable("UserStatistics");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.TotalWorkouts).HasDefaultValue(0);

            builder.Property(x => x.TotalCaloriesBurned).HasDefaultValue(0);

            builder.Property(x => x.CurrentStreak).HasDefaultValue(0);

            builder.Property(x => x.LongestStreak).HasDefaultValue(0);

            builder.Property(x => x.LatestWeight)
                   .HasPrecision(10, 2);

            builder.Property(x => x.StartingWeight)
                   .HasPrecision(10, 2);

            builder.Property(x => x.GoalWeight)
                   .HasPrecision(10, 2);

            //builder.Property(x => x.CreatedAt).IsRequired();
            //builder.Property(x => x.UpdatedAt).IsRequired();
        }

       
    }
}
