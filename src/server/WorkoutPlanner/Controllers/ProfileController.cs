using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutPlanner.Api.Repositories;
using WorkoutPlanner.Api.Services;
using WorkoutPlanner.Models.Requests;
using WorkoutPlanner.Models.Responses;


namespace WorkoutPlanner.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IProfileRepository _profileRepository;
    private readonly IValidator<UpdateProfileRequest> _updateRequestValidator;

    public ProfileController(IAuthService authService, IProfileRepository accountRepository, IValidator<UpdateProfileRequest> updateRequestValidator)
    {
        _authService = authService;
        _profileRepository = accountRepository;
        _updateRequestValidator = updateRequestValidator;
    }

    [HttpGet("{profileId}")]
    public async Task<ActionResult<ProfileResponse>> GetAccountAsync(
        int profileId
    )
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        if(userResult.AccountId != profileId)
            return Unauthorized("Cannot access information about diffrent accounts.");

        
        ProfileResponse response = await _profileRepository.GetProfileAsync(profileId);
        return Ok(response);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAccountAsync(
        [FromBody] UpdateProfileRequest request
    )
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);

        if(userResult.AccountId != request.ProfileId)
            return Unauthorized("Cannot update informaton about diffrent accounts.");

        _updateRequestValidator.ValidateAndThrow(request);

        if(string.IsNullOrEmpty(request.Email) == false && userResult.Email != request.Email)
            await _authService.UpdateEmailAsync(userResult.Email, request.Email);

        await _profileRepository.UpdateProfileAsync(request);
        return Ok();
    }
}
