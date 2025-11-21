using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Models;

namespace NutritionService.Infrastructure.Configurations
{
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.ToTable("Ingredients");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(i => i.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasMany(i => i.MealIngredients)
                   .WithOne(mi => mi.Ingredient)
                   .HasForeignKey(mi => mi.IngredientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
