namespace WorkoutPlanner.Models.Responses;

public class ProfileResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SurName { get; set; } = string.Empty;
    public int Age { get; set; } 
    public decimal Height { get; set; } 
    public decimal Weight { get; set; } 
}
