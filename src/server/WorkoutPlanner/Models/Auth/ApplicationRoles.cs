namespace WorkoutPlanner.Models.Auth;

public static class ApplicationRoles
{
    public const string Admin = "admin";
    public const string User = "user";
    public static string[] All = new string[] { Admin, User };

    public static readonly string[] AdminRoles = new[] { Admin };
    public static readonly string[] UserRoles = new[] { Admin, User };
}