using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Models;

namespace WorkoutPlanner.Database.Application;

public class WorkoutPlannerDatabaseContext : DbContext
{
    public WorkoutPlannerDatabaseContext(DbContextOptions options) : base(options) {}

    public DbSet<Profile> Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("WP");

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), type =>
        {
            string currentNamespace = typeof(WorkoutPlannerDatabaseContext).Namespace!;
            return type.Namespace?.StartsWith(currentNamespace, StringComparison.Ordinal) ?? false;
        });
    }
}
