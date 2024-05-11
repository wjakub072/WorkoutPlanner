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

        builder.Entity<ApplicationUser>().ToTable("Users", "auth");
        builder.Entity<ApplicationRole>().ToTable("Roles", "auth");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims", "auth");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims", "auth");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens", "auth");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins", "auth");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles", "auth");

        builder.ApplyConfigurationsFromAssembly(
                typeof(IdentityDatabaseContext).Assembly,
                    type => type.Namespace?.StartsWith(typeof(IdentityDatabaseContext).Namespace!,
                                                   StringComparison.OrdinalIgnoreCase)
                        ?? false);
    }
}
