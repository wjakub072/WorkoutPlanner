using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Database.Application.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingriedients", schema: "WP")
            .HasKey(x => x.Id);

        builder.Property(c => c.Calories).HasPrecision(11, 4);

    }
}
