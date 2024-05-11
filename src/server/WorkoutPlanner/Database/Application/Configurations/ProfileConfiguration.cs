using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Database.Application.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profiles", schema:"WP")
            .HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(255);
        builder.Property(c => c.SurName).HasMaxLength(255);
    }
}
