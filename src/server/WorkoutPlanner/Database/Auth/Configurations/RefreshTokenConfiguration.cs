using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkoutPlanner.Models.Auth;

namespace WorkoutPlanner.Database.Auth.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("Tokens", schema: "auth")
                .HasKey(c => c.Id);

        builder.HasAlternateKey(x => x.Token);

        builder.Property(x => x.Token).HasMaxLength(64);

        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.UserId);
    }
}
