using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Models.Auth;

namespace WorkoutPlanner.Database.Auth;

public class IdentityDatabaseContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public IdentityDatabaseContext(DbContextOptions<IdentityDatabaseContext> options) : base(options) { }

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users", "WPauth");
        builder.Entity<ApplicationRole>().ToTable("Roles", "WPauth");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "WPauth");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", "WPauth");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", "WPauth");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", "WPauth");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles", "WPauth");

        builder.ApplyConfigurationsFromAssembly(
                typeof(IdentityDatabaseContext).Assembly,
                    type => type.Namespace?.StartsWith(typeof(IdentityDatabaseContext).Namespace!,
                                                   StringComparison.OrdinalIgnoreCase)
                        ?? false);
    }
}
