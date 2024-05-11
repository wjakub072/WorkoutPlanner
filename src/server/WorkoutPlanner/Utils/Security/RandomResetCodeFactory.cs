namespace WorkoutPlanner.Api.Utils.Security;

public static class RandomResetCodeFactory
{
    private static readonly Random random = new Random();

    public static string GenerateRandomCode()
    {
        return random.Next(100000, 1000000).ToString("D6");
    }
}
