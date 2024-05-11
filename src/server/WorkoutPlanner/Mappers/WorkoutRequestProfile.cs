using AutoMapper;
using WorkoutPlanner.Models;
using WorkoutPlanner.Models.Enums;
using WorkoutPlanner.Models.Requests;

namespace WorkoutPlanner.Mappers;

public class WorkoutRequestProfile : AutoMapper.Profile
{
    public WorkoutRequestProfile()
    {
        CreateMap<ExerciseRequest, Exercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => DateTime.Parse(src.StartDateTime)))
            .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => DateTime.Parse(src.EndDateTime)))
            .ForMember(dest => dest.CaloriesBurned, opt => opt.MapFrom(src => decimal.Parse(src.CaloriesBurned)))
            .ForMember(dest => dest.Feeling, opt => opt.MapFrom(src => Enum.Parse<FeelingGrade>(src.Feeling)));

        CreateMap<UpdateExerciseRequest, Exercise>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => DateTime.Parse(src.StartDateTime)))
            .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => DateTime.Parse(src.EndDateTime)))
            .ForMember(dest => dest.CaloriesBurned, opt => opt.MapFrom(src => decimal.Parse(src.CaloriesBurned)))
            .ForMember(dest => dest.Feeling, opt => opt.MapFrom(src => Enum.Parse<FeelingGrade>(src.Feeling)));

        CreateMap<CreateWorkoutRequest, Workout>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => DateTime.Parse(src.StartDateTime)))
            .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => DateTime.Parse(src.EndDateTime)))
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(c => c.Exercises));

    }
}
