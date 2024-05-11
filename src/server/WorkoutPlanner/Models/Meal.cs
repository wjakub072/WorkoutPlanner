namespace WorkoutPlanner.Models;

public class Meal 
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public DateTime Date { get; set; }
    public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
}
