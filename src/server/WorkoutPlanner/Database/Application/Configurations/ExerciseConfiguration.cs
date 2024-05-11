using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WorkoutPlanner.Models;
using WorkoutPlanner.Models.Enums;

namespace WorkoutPlanner.Database.Application.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises", schema:"WP")
            .HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(255);
        builder.Property(c => c.Feeling)
                .HasConversion(new EnumToStringConverter<FeelingGrade>());
        builder.Property(c => c.CaloriesBurned).HasPrecision(11, 4);
    }
}
