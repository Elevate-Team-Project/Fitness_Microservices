using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entity;

namespace ProgressTrackingService.Infrastructure.Configrations
{
    public class WorkoutLogConfiguration : IEntityTypeConfiguration<WorkoutLog>
    {
        public void Configure(EntityTypeBuilder<WorkoutLog> builder)
        {
            builder.ToTable("WorkoutLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.WorkoutId).IsRequired();

            builder.Property(x => x.WorkoutName)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(x => x.SessionId)
                   .HasMaxLength(50);

            builder.Property(x => x.CaloriesBurned).IsRequired();

            builder.Property(x => x.Duration).IsRequired();

            builder.Property(x => x.Rating);

            builder.Property(x => x.Notes)
                   .HasMaxLength(500);

            
           
        }
    }
}
