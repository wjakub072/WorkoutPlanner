namespace WorkoutPlanner.Models.Requests;

public class UpdateExerciseRequest 
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDateTime { get; set; } = string.Empty;
    public string EndDateTime { get; set; } = string.Empty;
    public string CaloriesBurned { get; set; } = string.Empty;
    public string Feeling { get; set; } = string.Empty;
}