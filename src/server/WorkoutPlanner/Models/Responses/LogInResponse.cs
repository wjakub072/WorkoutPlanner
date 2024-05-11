using WorkoutPlanner.Models.Responses;

namespace WorkoutPlanner.Api.Data.Responses;

public class LogInResponse
{
    public UserResponse? User { get; set; }

    public TokenResponse? AccessToken { get; set; }
}
