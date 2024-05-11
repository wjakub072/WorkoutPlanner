using AutoMapper;
using WorkoutPlanner.Models;
using WorkoutPlanner.Models.Responses;

namespace WorkoutPlanner.Mappers;

public class WorkoutProfile : AutoMapper.Profile
{
    public WorkoutProfile()
    {
       CreateMap<Workout, WorkoutResponse>()
            .ForMember(dest => dest.Exercises, opt => opt.MapFrom(src => src.Exercises));

        CreateMap<Exercise, ExerciseResponse>()
            .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => src.StartDateTime.ToString()))
            .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => src.EndDateTime.ToString()))
            .ForMember(dest => dest.Feeling, opt => opt.MapFrom(src => src.Feeling.ToString()));
    }
}
