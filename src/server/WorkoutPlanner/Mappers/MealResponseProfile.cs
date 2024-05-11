using WorkoutPlanner.Models;
using WorkoutPlanner.Models.Responses;

namespace WorkoutPlanner.Mappers;

public class MealResponseProfile : AutoMapper.Profile
{
    public MealResponseProfile()
    {
       CreateMap<Meal, MealResponse>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.Date.ToString()))
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

        CreateMap<Ingredient, IngredientResponse>()
            .ForMember(dest => dest.Calories, opt => opt.MapFrom(src => src.Calories.ToString()));
    }
}
