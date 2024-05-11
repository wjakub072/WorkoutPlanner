namespace WorkoutPlanner.Models;

public class Ingredient
{
    public int MealId { get; internal set; }
    public string Name { get; set; } = default!;
    public decimal Calories { get; set; }
}
