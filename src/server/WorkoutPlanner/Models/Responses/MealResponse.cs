namespace WorkoutPlanner.Models.Responses;

public class MealResponse
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public string Date { get; set; } = string.Empty;
    public IngredientResponse[] Ingredients { get; set; } = Array.Empty<IngredientResponse>();
}
