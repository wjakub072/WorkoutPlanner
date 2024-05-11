namespace WorkoutPlanner.Models.Responses;

public class WorkoutResponse
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public string StartDateTime { get; set; } = string.Empty;
    public string EndDateTime { get; set; } = string.Empty;

    public ExerciseResponse[] Exercises { get; set; } = Array.Empty<ExerciseResponse>();
}
