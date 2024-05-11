namespace WorkoutPlanner.Models.Requests;

public class CreateWorkoutRequest
{
    public int ProfileId { get; set; }
    public string StartDateTime { get; set; } = string.Empty;
    public string EndDateTime { get; set; } = string.Empty;

    public ExerciseRequest[] Exercises { get; set; } = Array.Empty<ExerciseRequest>();
}
