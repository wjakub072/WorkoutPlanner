using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutPlanner.Api.Services;
using WorkoutPlanner.Models.Requests;
using WorkoutPlanner.Models.Responses;
using WorkoutPlanner.Repositories;

namespace WorkoutPlanner.Controllers;

[ApiController]
[Authorize]
[Route("api/profile/meals")]
public class MealController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMealRepository _mealRepository;

    public MealController(IAuthService authService, IMealRepository mealRepository)
    {
        _authService = authService;
        _mealRepository = mealRepository;
    }

    [HttpGet()]
    public async Task<ActionResult<MealResponse[]>> GetMealsAsync()
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        var result = await _mealRepository.GetMealsAsync(userResult.AccountId);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MealResponse[]>> GetMealAsync(int id)
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        var result = await _mealRepository.GetMealAsync(userResult.AccountId, id);

        return Ok(result);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateMealAsync(
        [FromBody] CreateMealRequest request
    )
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        if (request.ProfileId != userResult.AccountId) return Forbid("Wrong profileId, you cant add workouts to diffrent users");

        await _mealRepository.CreateMealAsync(request);

        return Ok();
    }

    [HttpPut()]
    public async Task<IActionResult> UpdateWorkoutAsync(
        [FromBody] UpdateMealRequest request
    )
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        if (request.ProfileId != userResult.AccountId) return Forbid("Wrong profileId, you cant add workouts to diffrent users");

        await _mealRepository.UpdateMealAsync(userResult.AccountId, request);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMealAsync(int id)
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        await _mealRepository.DeleteMealAsync(userResult.AccountId, id);

        return Ok();
    }
}
