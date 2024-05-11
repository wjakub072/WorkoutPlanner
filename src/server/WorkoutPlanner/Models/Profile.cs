namespace WorkoutPlanner.Models;

public class Profile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SurName { get; set; } = string.Empty;
    public int Age { get; set; } 
    public decimal Height { get; set; } 
    public decimal Weight { get; set; } 
}