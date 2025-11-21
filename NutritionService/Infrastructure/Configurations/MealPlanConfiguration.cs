using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Models;

namespace NutritionService.Infrastructure.Configurations
{
    public class MealPlanConfiguration : IEntityTypeConfiguration<MealPlan>
    {
        public void Configure(EntityTypeBuilder<MealPlan> builder)
        {
            builder.ToTable("MealPlans");

            builder.HasKey(mp => mp.Id);

            builder.Property(mp => mp.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(mp => mp.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(mp => mp.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(mp => mp.CalorieTarget)
                   .IsRequired();

            builder.HasMany(mp => mp.Meals)
                   .WithOne(m => m.MealPlan)
                   .HasForeignKey(m => m.MealPlanId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
