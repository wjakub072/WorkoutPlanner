using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Database.Application;
using WorkoutPlanner.Models.Requests;
using WorkoutPlanner.Models;
using WorkoutPlanner.Models.Responses;
using WorkoutPlanner.Models.Enums;

namespace WorkoutPlanner.Repositories;

public interface IWorkoutRepository {
    public Task<WorkoutResponse[]> GetWorkoutsAsync(int profileId);
    public Task CreateWorkoutAsync(CreateWorkoutRequest request);
    public Task<WorkoutResponse> GetWorkoutAsync(int profileId, int workoutId);
    public Task DeleteWorkoutAsync(int profileId, int workoutId);
    public Task DeleteExerciseAsync(int profileId, int exerciseId);
    public Task UpdateWorkoutAsync(int profileId, UpdateWorkoutRequest request);
}

public class WorkoutRepository : IWorkoutRepository 
{
    private readonly WorkoutPlannerDatabaseContext _db;
    private readonly AutoMapper.IMapper _mapper;

    public WorkoutRepository(WorkoutPlannerDatabaseContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<WorkoutResponse[]> GetWorkoutsAsync(int profileId)
    {
        var workouts = await _db.Workouts.Where(c => c.ProfileId == profileId).Include(c => c.Exercises).ToListAsync();
        
        ICollection<WorkoutResponse> result = new List<WorkoutResponse>();
        
        foreach (var workout in workouts)
        {
            result.Add(_mapper.Map<WorkoutResponse>(workout));
        }

        return result.ToArray();
    }

    public async Task<WorkoutResponse> GetWorkoutAsync(int profileId, int workoutId)
    {
        var workout = await _db.Workouts.Where(c => c.Id == workoutId).Include(c => c.Exercises).FirstOrDefaultAsync();
        
        if(workout is null) throw new Exception("Workout not found.");
        if(workout.ProfileId != profileId) throw new Exception("Cannot access other user workouts.");

        WorkoutResponse result = _mapper.Map<WorkoutResponse>(workout);

        return result;
    }

    public async Task CreateWorkoutAsync(CreateWorkoutRequest request)
    {
        Models.Profile? profile = await _db.Profiles.FirstOrDefaultAsync(a => a.Id == request.ProfileId);

        if(profile is null)
            throw new Exception($"Profile not found for id: {request.ProfileId}");

        foreach (var exercise in request.Exercises)
            exercise.CaloriesBurned = exercise.CaloriesBurned.Replace(".", ",");

        var workout = _mapper.Map<Workout>(request);

        _db.Workouts.Add(workout);

        await _db.SaveChangesAsync();
    }

    public async Task DeleteWorkoutAsync(int profileId, int workoutId)
    {
        var workout = await _db.Workouts.Where(c => c.Id == workoutId).Include(c => c.Exercises).FirstOrDefaultAsync();

        if(workout is null) throw new Exception("Workout not found.");
        if(workout.ProfileId != profileId) throw new Exception("Cannot access other user workouts.");

        _db.Workouts.Remove(workout);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteExerciseAsync(int profileId, int exerciseId)
    {
        var exercise = await _db.Exercises.Where(c => c.Id == exerciseId).FirstOrDefaultAsync();

        if(exercise is null) throw new Exception("Exercise not found.");

        var workout = await _db.Workouts.Where(c => c.Id == exercise.WorkoutId).FirstOrDefaultAsync();
        if(workout is null) throw new Exception("Workout for exercise not found.");
        if(workout.ProfileId != profileId) throw new Exception("Cannot access other user workouts.");

        _db.Exercises.Remove(exercise);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateWorkoutAsync(int profileId, UpdateWorkoutRequest request)
    {
        var workout = await _db.Workouts.Where(c => c.Id == request.Id).Include(c => c.Exercises).FirstOrDefaultAsync();

        if(workout is null) throw new Exception("Workout not found.");
        if(workout.ProfileId != profileId) throw new Exception("Cannot access other user workouts.");

        if (string.IsNullOrEmpty(request.StartDateTime) == false 
        && workout.StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") != request.StartDateTime){
            workout.StartDateTime = DateTime.Parse(request.StartDateTime);
        }

        if (string.IsNullOrEmpty(request.EndDateTime) == false 
        && workout.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") != request.EndDateTime){
            workout.EndDateTime = DateTime.Parse(request.EndDateTime);
        }

        var existingRequestExercises = request.Exercises.Where(c => c.Id is not null).ToList();
        foreach(var exerciseRequest in existingRequestExercises)
        {
            var exercise = workout.Exercises.FirstOrDefault(c => c.Id == exerciseRequest.Id);

            if(exercise is null) continue; // Be careful here

            if (string.IsNullOrEmpty(exerciseRequest.Name) == false)
                exercise.Name = exerciseRequest.Name;

            if (string.IsNullOrEmpty(exerciseRequest.StartDateTime) == false)
                exercise.StartDateTime = DateTime.Parse(exerciseRequest.StartDateTime);

            if (string.IsNullOrEmpty(exerciseRequest.EndDateTime) == false)
                exercise.EndDateTime = DateTime.Parse(exerciseRequest.EndDateTime);

            if (string.IsNullOrEmpty(exerciseRequest.CaloriesBurned) == false)
                exercise.CaloriesBurned = decimal.Parse(exerciseRequest.CaloriesBurned.Replace(".", ","));

            if (string.IsNullOrEmpty(exerciseRequest.Feeling) == false)
                exercise.Feeling = Enum.Parse<FeelingGrade>(exerciseRequest.Feeling);
        }

        var allRequestExercisesIds = existingRequestExercises.Select(c => c.Id).ToList();
        var deletedExercieses = workout.Exercises.Where(c => allRequestExercisesIds.Contains(c.Id) == false).ToList();

        foreach (var deletedExcercise in deletedExercieses){
            workout.Exercises.Remove(deletedExcercise);
        }

        var newExercises = request.Exercises.Where(c => c.Id is null).ToList();

        foreach (var exerciseRequest in newExercises){
            exerciseRequest.CaloriesBurned = exerciseRequest.CaloriesBurned.Replace(".", ",");
            var exercise = _mapper.Map<Exercise>(exerciseRequest);

            workout.Exercises.Add(exercise);
        }

        await _db.SaveChangesAsync();
    }
}
