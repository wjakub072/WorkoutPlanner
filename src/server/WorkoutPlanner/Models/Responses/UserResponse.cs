namespace WorkoutPlanner.Models.Responses;

public class UserResponse
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string UserName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool IsLocked => LockoutEnd > DateTimeOffset.UtcNow;
}

