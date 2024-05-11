namespace WorkoutPlanner.Models.Requests;

public class UpdateMealRequest
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public string Date { get; set; } = string.Empty;

    public UpdateIngredientRequest[] Ingredients { get; set; } = Array.Empty<UpdateIngredientRequest>();
}
