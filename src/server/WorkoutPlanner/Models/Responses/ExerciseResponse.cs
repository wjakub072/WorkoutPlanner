namespace WorkoutPlanner.Models.Responses;

public class ExerciseResponse
{
    public int Id { get; set; }
    public int WorkoutId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDateTime { get; set; } = string.Empty;
    public string EndDateTime { get; set; } = string.Empty;
    public decimal CaloriesBurned { get; set; }
    public string Feeling { get; set; } = string.Empty;
}
