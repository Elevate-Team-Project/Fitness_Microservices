using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressTrackingService.Domain.Entity;

namespace ProgressTrackingService.Infrastructure.Configrations
{
    public class WeightHistoryConfiguration : IEntityTypeConfiguration<WeightHistory>
    {
        public void Configure(EntityTypeBuilder<WeightHistory> builder)
        {
            builder.ToTable("WeightHistory");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();

            builder.Property(x => x.Weight)
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.Property(x => x.LoggedAt).IsRequired();

            //builder.Property(x => x.CreatedAt).IsRequired();
            //builder.Property(x => x.UpdatedAt).IsRequired();
        }
    }
}
