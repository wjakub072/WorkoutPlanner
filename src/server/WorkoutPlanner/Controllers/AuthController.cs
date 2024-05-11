using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfitNest.Api.Data.Requests;
using ProfitNest.Api.Data.Responses;
using ProfitNest.Api.Services;

namespace ProfitNest.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<SignUpRequest> _signUpRequestValidator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;

    public AuthController(IAuthService authService, IValidator<SignUpRequest> signUpRequestValidator, IValidator<LoginRequest> loginRequestValidator)
    {
        _authService = authService;
        _signUpRequestValidator = signUpRequestValidator;
        _loginRequestValidator = loginRequestValidator;
    }

    [HttpPost("log-in")]
    [AllowAnonymous]
    public async Task<IActionResult> LogInAsync(
        [FromBody] LoginRequest request)
    {
        _loginRequestValidator.ValidateAndThrow(request);

        var (accessTokenResponse, refreshTokenResponse) = await _authService.LogInAsync(request);

        LogInResponse logInResponse = new()
        {
            AccessToken = accessTokenResponse,
            User = await _authService.FindCurrentUserByNameAsync(request.UserName)
        };

        CookieOptions cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenResponse.TokenExpires
        };
        Response.Cookies.Append("refreshToken", refreshTokenResponse.Token, cookieOptions);       

        return Ok(logInResponse);
    }

    [HttpPost("log-out")]
    [AllowAnonymous]
    public async Task<IActionResult> LogOutAsync()
    {
        string? refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return NoContent();

        await _authService.LogOutAsync(refreshToken);
        return NoContent();
    }


    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshTokenAsync()
    {
        string? refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest();

        var (accessTokenResponse, refreshTokenResponse) = await _authService.CreateTokensFromRefreshTokenAsync(refreshToken);

        CookieOptions cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenResponse.TokenExpires
        };
        Response.Cookies.Append("refreshToken", refreshTokenResponse.Token, cookieOptions);

        return Ok(accessTokenResponse);
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUpAsync(
        [FromBody] SignUpRequest credentials)
    {
        _signUpRequestValidator.ValidateAndThrow(credentials);

        var userResult = await _authService.SignUpAsync(credentials);
        return Ok(userResult);
    }

    [HttpGet("user")]
    public async Task<ActionResult<UserResponse>> GetCurrentUserAsync()
    {
        string userName = User.Identity?.Name ?? "";
        if (userName.Length == 0) return Forbid();

        UserResponse userResult = await _authService.FindCurrentUserByNameAsync(userName);
        return Ok(userResult);
    }
}