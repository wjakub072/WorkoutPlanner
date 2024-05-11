namespace WorkoutPlanner.Models.Requests;

public class CreateMealRequest
{
    public int ProfileId { get; set; }
    public string Date { get; set; } = string.Empty;

    public IngredientRequest[] Ingredients { get; set; } = Array.Empty<IngredientRequest>();
}
