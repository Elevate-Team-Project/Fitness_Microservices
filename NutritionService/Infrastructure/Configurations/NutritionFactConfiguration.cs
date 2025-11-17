using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Models;

namespace NutritionService.Infrastructure.Configurations
{
    public class NutritionFactConfiguration : IEntityTypeConfiguration<NutritionFact>
    {
        public void Configure(EntityTypeBuilder<NutritionFact> builder)
        {
            builder.Property(n => n.Calories)
              .IsRequired();

            builder.Property(n => n.Protein)
                   .IsRequired();

            builder.Property(n => n.Carbs)
                   .IsRequired();

            builder.Property(n => n.Fats)
                   .IsRequired();

            builder.Property(n => n.Fiber)
                   .IsRequired();

            builder.HasOne(n => n.Meal)
                   .WithOne(m => m.NutritionFacts)
                   .HasForeignKey<NutritionFact>(n => n.MealId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
