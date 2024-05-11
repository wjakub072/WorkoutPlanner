namespace WorkoutPlanner.Models.Requests;

public class UpdateIngredientRequest
{
    public int? Id { get; set; }
    public int MealId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Calories { get; set; } = string.Empty;
}