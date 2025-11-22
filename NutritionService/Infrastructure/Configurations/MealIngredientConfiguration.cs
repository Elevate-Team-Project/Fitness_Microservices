using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Models;

namespace NutritionService.Infrastructure.Configurations
{
    public class MealIngredientConfiguration : IEntityTypeConfiguration<MealIngredient>
    {
        public void Configure(EntityTypeBuilder<MealIngredient> builder)
        {
            builder.ToTable("MealIngredients");

            builder.HasKey(mi => mi.Id);

            builder.Property(mi => mi.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(mi => mi.Amount)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasOne(mi => mi.Meal)
                   .WithMany(m => m.MealIngredients)
                   .HasForeignKey(mi => mi.MealId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mi => mi.Ingredient)
                   .WithMany(i => i.MealIngredients)
                   .HasForeignKey(mi => mi.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(mi => new { mi.MealId, mi.IngredientId })
                   .IsUnique();
        }
    }
}
