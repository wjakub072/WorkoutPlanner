using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Database.Application;
using WorkoutPlanner.Models.Requests;
using WorkoutPlanner.Models;
using WorkoutPlanner.Models.Responses;
using WorkoutPlanner.Models.Enums;

namespace WorkoutPlanner.Repositories;

public interface IMealRepository {
    public Task<MealResponse[]> GetMealsAsync(int profileId);
    public Task<MealResponse> GetMealAsync(int profileId, int mealId);
    public Task CreateMealAsync(CreateMealRequest request);
    public Task DeleteMealAsync(int profileId, int mealId);
    public Task DeleteIngredientAsync(int profileId, int ingriedientId);
    public Task UpdateMealAsync(int profileId, UpdateMealRequest request);
}

public class MealRepository : IMealRepository 
{
    private readonly WorkoutPlannerDatabaseContext _db;
    private readonly AutoMapper.IMapper _mapper;

    public MealRepository(WorkoutPlannerDatabaseContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task CreateMealAsync(CreateMealRequest request)
    {
        Models.Profile? profile = await _db.Profiles.FirstOrDefaultAsync(a => a.Id == request.ProfileId);

        if(profile is null)
            throw new Exception($"Profile not found for id: {request.ProfileId}");

        foreach (var ingredient in request.Ingredients)
            ingredient.Calories = ingredient.Calories.Replace(".", ",");

        var meal = _mapper.Map<Meal>(request);

        _db.Meals.Add(meal);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteIngredientAsync(int profileId, int ingriedientId)
    {
        var ingredient = await _db.Ingredients.Where(c => c.Id == ingriedientId).FirstOrDefaultAsync();

        if(ingredient is null) throw new Exception("Ingredient not found.");

        var workout = await _db.Meals.Where(c => c.Id == ingredient.MealId).FirstOrDefaultAsync();
        if(workout is null) throw new Exception("Meal for ingredient not found.");
        if(workout.ProfileId != profileId) throw new Exception("Cannot access other user meals.");

        _db.Ingredients.Remove(ingredient);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteMealAsync(int profileId, int mealId)
    {
        var meal = await _db.Meals.Where(c => c.Id == mealId).Include(c => c.Ingredients).FirstOrDefaultAsync();

        if(meal is null) throw new Exception("Meal not found.");
        if(meal.ProfileId != profileId) throw new Exception("Cannot access other user meals.");

        _db.Meals.Remove(meal);
        await _db.SaveChangesAsync();
    }

    public async Task<MealResponse> GetMealAsync(int profileId, int mealId)
    {
        var meal = await _db.Meals.Where(c => c.Id == mealId).Include(c => c.Ingredients).FirstOrDefaultAsync();
        
        if(meal is null) throw new Exception("Meal not found.");
        if(meal.ProfileId != profileId) throw new Exception("Cannot access other user meals.");

        MealResponse result = _mapper.Map<MealResponse>(meal);

        return result;
    }

    public async Task<MealResponse[]> GetMealsAsync(int profileId)
    {
        var meals = await _db.Meals.Where(c => c.ProfileId == profileId).Include(c => c.Ingredients).ToListAsync();
        
        ICollection<MealResponse> result = new List<MealResponse>();
        
        foreach (var workout in meals)
        {
            result.Add(_mapper.Map<MealResponse>(workout));
        }

        return result.ToArray();
    }

    public async Task UpdateMealAsync(int profileId, UpdateMealRequest request)
    {
        var meal = await _db.Meals.Where(c => c.Id == request.Id).Include(c => c.Ingredients).FirstOrDefaultAsync();

        if(meal is null) throw new Exception("Meal not found.");
        if(meal.ProfileId != profileId) throw new Exception("Cannot access other user meals.");

        if (string.IsNullOrEmpty(request.Date) == false 
        && meal.Date.ToString("yyyy-MM-dd") != request.Date){
            meal.Date = DateTime.Parse(request.Date).Date;
        }

        var existingRequestIngredients = request.Ingredients.Where(c => c.Id is not null).ToList();
        foreach(var ingredientRequest in existingRequestIngredients)
        {
            var ingredient = meal.Ingredients.FirstOrDefault(c => c.Id == ingredientRequest.Id);

            if(ingredient is null) continue; // Be careful here

            if (string.IsNullOrEmpty(ingredientRequest.Name) == false)
                ingredient.Name = ingredientRequest.Name;

            if (string.IsNullOrEmpty(ingredientRequest.Calories) == false)
                ingredient.Calories = decimal.Parse(ingredientRequest.Calories.Replace(".", ","));
        }

        var allRequestIngredientsIds = existingRequestIngredients.Select(c => c.Id).ToList();
        var deletedIngriedients = meal.Ingredients.Where(c => allRequestIngredientsIds.Contains(c.Id) == false).ToList();

        foreach (var deletedIngredients in deletedIngriedients){
            meal.Ingredients.Remove(deletedIngredients);
        }

        var newExercises = request.Ingredients.Where(c => c.Id is null).ToList();

        foreach (var exerciseRequest in newExercises){
            exerciseRequest.Calories = exerciseRequest.Calories.Replace(".", ",");
            var ingriedient = _mapper.Map<Ingredient>(exerciseRequest);

            meal.Ingredients.Add(ingriedient);
        }

        await _db.SaveChangesAsync();
    }
}
