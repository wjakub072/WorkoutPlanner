using WorkoutPlanner.Models;
using WorkoutPlanner.Models.Requests;

namespace WorkoutPlanner.Mappers;

public class MealRequestProfile : AutoMapper.Profile
{
    public MealRequestProfile()
    {
        CreateMap<IngredientRequest, Ingredient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.Calories, opt => opt.MapFrom(src => decimal.Parse(src.Calories)));

        CreateMap<UpdateIngredientRequest, Ingredient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.Calories, opt => opt.MapFrom(src => decimal.Parse(src.Calories)));

        CreateMap<CreateMealRequest, Meal>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Parse(src.Date)))
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(c => c.Ingredients));
    }
    
}
