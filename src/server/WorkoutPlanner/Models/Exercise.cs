using WorkoutPlanner.Models.Enums;

namespace WorkoutPlanner.Models;

public class Exercise
{
    public int Id { get; set; }
    public int WorkoutId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public decimal CaloriesBurned { get; set; }
    public FeelingGrade Feeling { get; set; }
}