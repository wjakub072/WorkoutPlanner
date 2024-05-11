namespace WorkoutPlanner.Models.Requests;

public class UpdateWorkoutRequest
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public string StartDateTime { get; set; } = string.Empty;
    public string EndDateTime { get; set; } = string.Empty;

    public UpdateExerciseRequest[] Exercises { get; set; } = Array.Empty<UpdateExerciseRequest>();
}
