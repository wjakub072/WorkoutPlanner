using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Database.Application.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.ToTable("Workouts", schema:"WP")
            .HasKey(c => c.Id);

        builder.HasMany(c => c.Exercises)
            .WithOne()
            .HasForeignKey(c => c.WorkoutId);
    }
}
