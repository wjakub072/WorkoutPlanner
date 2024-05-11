namespace WorkoutPlanner.Models.Responses;

public class TokenResponse
{
    public string Token { get; init; } = string.Empty;
    
    public DateTime TokenExpires { get; init; }
}
