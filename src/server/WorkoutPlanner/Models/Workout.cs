using System.Collections.ObjectModel;

namespace WorkoutPlanner.Models;

public class Workout
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    public ICollection<Exercise> Exercises { get; set; } = new Collection<Exercise>();
}
