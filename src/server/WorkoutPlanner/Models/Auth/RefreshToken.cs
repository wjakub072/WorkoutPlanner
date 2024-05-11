namespace WorkoutPlanner.Models.Auth;

public class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = "";

    public DateTime Expires { get; set; }

    public bool IsExpired => DateTime.Now >= Expires;

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime? Revoked { get; set; }

    public bool IsActive => Revoked == null && !IsExpired;
}
