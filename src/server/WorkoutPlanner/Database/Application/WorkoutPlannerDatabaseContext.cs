using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace WorkoutPlanner.Database.Application;

public class WorkoutPlannerDatabaseContext : DbContext
{
    public WorkoutPlannerDatabaseContext(DbContextOptions options) : base(options) {}

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
