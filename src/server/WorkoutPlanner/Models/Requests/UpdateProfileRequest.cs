namespace WorkoutPlanner.Models.Requests;

public class UpdateProfileRequest
{
    public int ProfileId { get; set; }
    public string? Email { get; set; } 
    public string? Name { get; set; } 
    public string? SurName { get; set; } 
    public int? Age { get; set; } 
    public string? Height { get; set; } 
    public string? Weight { get; set; } 
}
