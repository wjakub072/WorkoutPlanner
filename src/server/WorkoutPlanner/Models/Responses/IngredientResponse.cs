namespace WorkoutPlanner.Models.Responses;

public class IngredientResponse
{
    public int Id { get; set; }
    public int MealId { get;  set; }
    public string Name { get; set; } = string.Empty;
    public decimal Calories { get; set; } 
}