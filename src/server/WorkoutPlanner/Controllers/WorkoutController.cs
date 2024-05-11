using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutPlanner.Api.Services;
using WorkoutPlanner.Models.Requests;
using WorkoutPlanner.Models.Responses;
using WorkoutPlanner.Repositories;

namespace WorkoutPlanner.Controllers;

[ApiController]
[Authorize]
[Route("api/profile/workouts")]
public class WorkoutController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWorkoutRepository _workoutRepository;

    public WorkoutController(IAuthService authService, IWorkoutRepository workoutRepository)
    {
        _authService = authService;
        _workoutRepository = workoutRepository;
    }

    [HttpGet()]
    public async Task<ActionResult<WorkoutResponse[]>> GetWorkoutsAsync()
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        var result = await _workoutRepository.GetWorkoutsAsync(userResult.AccountId);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkoutResponse[]>> GetWorkoutAsync(int id)
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        var result = await _workoutRepository.GetWorkoutAsync(userResult.AccountId, id);

        return Ok(result);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateWorkoutAsync(
        [FromBody] CreateWorkoutRequest request
    )
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        if (request.ProfileId != userResult.AccountId) return Forbid("Wrong profileId, you cant add workouts to diffrent users");

        await _workoutRepository.CreateWorkoutAsync(request);

        return Ok();
    }

    [HttpPut()]
    public async Task<IActionResult> UpdateWorkoutAsync(
        [FromBody] UpdateWorkoutRequest request
    )
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        if (request.ProfileId != userResult.AccountId) return Forbid("Wrong profileId, you cant add workouts to diffrent users");

        await _workoutRepository.UpdateWorkoutAsync(userResult.AccountId, request);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkoutAsync(int id)
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        await _workoutRepository.DeleteWorkoutAsync(userResult.AccountId, id);

        return Ok();
    }
}
