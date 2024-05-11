using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutPlanner.Api.Services;
using WorkoutPlanner.Database.Application;
using WorkoutPlanner.Models;
using WorkoutPlanner.Models.Requests;
using WorkoutPlanner.Models.Responses;
namespace WorkoutPlanner.Api.Repositories;

public interface IProfileRepository
{
    public Task<Profile> CreateBlankProfileAsync(string userName, string email);
    public Task<ProfileResponse> GetProfileAsync(int accountId);
    public Task UpdateProfileAsync(UpdateProfileRequest updateRequest);
}

public class ProfileRepository : IProfileRepository
{
    private readonly WorkoutPlannerDatabaseContext _db;
    private readonly ILogger<IProfileRepository> _log;

    public ProfileRepository(WorkoutPlannerDatabaseContext db, ILogger<IProfileRepository> log)
    {
        _db = db;
        _log = log;
    }

    public async Task<Profile> CreateBlankProfileAsync(string userName, string email)
    {
        Profile blank = new (){
            UserName = userName,
            Email = email
        };

        _db.Profiles.Add(blank);

        await _db.SaveChangesAsync().ConfigureAwait(false);

        _log.LogTrace("Blank user:{id} has been added, all his properties are set to default.", blank.Id);

        return blank;
    }    

    public async Task<ProfileResponse> GetProfileAsync(int accountId)
    {
        Profile? profile = await _db.Profiles.FirstOrDefaultAsync(a => a.Id == accountId);

        if(profile is null)
            throw new Exception($"Account not found for id: {accountId}");


        return CreateAccountResponse(profile);
    }

    private ProfileResponse CreateAccountResponse(Profile source)
        => new ProfileResponse(){
            Id = source.Id,
            UserName = source.UserName,
            Name = source.Name,
            SurName = source.SurName,
            Email = source.Email,
            Age = source.Age,
            Height = source.Height,
            Weight = source.Weight,
        };

    public async Task UpdateProfileAsync(UpdateProfileRequest updateRequest)
    {
        Profile? profile = await _db.Profiles.FirstOrDefaultAsync(a => a.Id == updateRequest.ProfileId);

        if(profile is null)
            throw new Exception($"Profile not found for id: {updateRequest.ProfileId}");

        if (string.IsNullOrEmpty(updateRequest.Name) == false)
            profile.Name = updateRequest.Name;

        if (string.IsNullOrEmpty(updateRequest.SurName) == false)
            profile.SurName = updateRequest.SurName;

        if (string.IsNullOrEmpty(updateRequest.Email) == false)
            profile.Email = updateRequest.Email;

        if (updateRequest.Age is not null)
            profile.Age = (int)updateRequest.Age;

        if (updateRequest.Height is not null)
            profile.Height = decimal.Parse(updateRequest.Height.Replace(".", ","));

        if (updateRequest.Weight is not null)
            profile.Weight = decimal.Parse(updateRequest.Weight.Replace(".", ","));


        await _db.SaveChangesAsync();
    }
}
