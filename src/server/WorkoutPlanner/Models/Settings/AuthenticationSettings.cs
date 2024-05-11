namespace WorkoutPlanner.Api.Data.Settings;

public class AuthenticationSettings
{
    public string Key { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public int AccessTokenDurationInMinutes { get; set; }
    
    public int RefreshTokenDurationInDays { get; set; }
}
