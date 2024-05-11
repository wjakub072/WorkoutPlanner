using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using WorkoutPlanner.Api.Data.Requests;
using WorkoutPlanner.Api.Data.Settings;
using WorkoutPlanner.Api.Repositories;
using WorkoutPlanner.Database.Auth;
using WorkoutPlanner.Exceptions;
using WorkoutPlanner.Models.Auth;
using WorkoutPlanner.Models.Responses;

namespace WorkoutPlanner.Api.Services;

public interface IAuthService
{
    public Task<(TokenResponse, TokenResponse)> LogInAsync(LoginRequest request);
    public Task LogOutAsync(string token);
    public Task<(TokenResponse, TokenResponse)> CreateTokensFromRefreshTokenAsync(string refreshToken);
    public Task<UserResponse> FindCurrentUserByNameAsync(string userName);
    public Task<UserResponse> FindCurrentUserByEmailAsync(string email);
    public Task<UserResponse> SignUpAsync(SignUpRequest payload);
    public Task UpdateEmailAsync(string email, string newEmail);
    public Task UpdateUserNameAsync(string userName, string newUserName);
}

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IdentityDatabaseContext _authDatabase;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IProfileRepository _accountRepository;

    public AuthService(ILogger<AuthService> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<AuthenticationSettings> authenticationSettings,
        IdentityDatabaseContext authDatabase,
        IProfileRepository accountRepository)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _authDatabase = authDatabase;
        _authenticationSettings = authenticationSettings.Value;
        _accountRepository = accountRepository;
    }

    public async Task<(TokenResponse, TokenResponse)> LogInAsync(LoginRequest request)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user is null ) throw new EndUserException("Incorrect email or password.");

        SignInResult attempt = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (attempt.IsLockedOut || attempt.IsNotAllowed)
            throw new EndUserException("Account has been blocked.");

        if (!attempt.Succeeded)
            throw new EndUserException("Incorrect email or password.");

        return await CreateTokensResultAsync(user);
    }

    public async Task LogOutAsync(string token)
    {
        RefreshToken? refreshToken = await _authDatabase.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);

        if (refreshToken is null)
            return;

        refreshToken.Revoked = DateTime.Now;
        await _authDatabase.SaveChangesAsync();
    }

    public async Task<(TokenResponse, TokenResponse)> CreateTokensFromRefreshTokenAsync(string token)
    {
        RefreshToken? refreshToken = await _authDatabase.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);

        if (refreshToken == null)
            throw new Exception("Refresh token validation failed. Token not found.");
        if (!refreshToken.IsActive)
            throw new Exception("Refresh token validation failed. Token inactive.");

        ApplicationUser? user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString(CultureInfo.InvariantCulture));
        if (user == null || await _userManager.IsLockedOutAsync(user))
            throw new Exception("Refresh token validation failed. User is locked.");

        refreshToken.Revoked = DateTime.Now;
        await _authDatabase.SaveChangesAsync();

        return await CreateTokensResultAsync(user);
    }

    private async Task<(TokenResponse, TokenResponse)> CreateTokensResultAsync(ApplicationUser user)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        JwtSecurityToken accessToken = await GenerateAccessTokenAsync(user);
        RefreshToken refreshToken = await GenerateRefreshTokenAsync(user);

        TokenResponse accessTokenResponse = new()
        {
            Token = tokenHandler.WriteToken(accessToken),
            TokenExpires = accessToken.ValidTo
        };
        TokenResponse refreshTokenResponse = new()
        {
            Token = refreshToken.Token,
            TokenExpires = refreshToken.Expires
        };

        return (accessTokenResponse, refreshTokenResponse);
    }

    public async Task<JwtSecurityToken> GenerateAccessTokenAsync(ApplicationUser user)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);

        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!.ToString()),
            new Claim(ClaimTypes.Role, roles.First()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.Key));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(_authenticationSettings.AccessTokenDurationInMinutes);

        return new JwtSecurityToken(
            _authenticationSettings.Issuer,
            _authenticationSettings.Issuer,
            claims,
            expires: expires,
            signingCredentials: cred);
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user)
    {
        using RandomNumberGenerator generator = RandomNumberGenerator.Create();
        byte[] randomNumber = new byte[32];
        generator.GetBytes(randomNumber);

        RefreshToken refreshToken = new()
        {
            Token = Convert.ToBase64String(randomNumber),
            Expires = DateTime.Now.AddDays(_authenticationSettings.RefreshTokenDurationInDays),
            Created = DateTime.Now,
            UserId = user.Id
        };

        await _authDatabase.RefreshTokens.AddAsync(refreshToken);
        await _authDatabase.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<UserResponse> FindCurrentUserByNameAsync(string userName)
    {
        ApplicationUser user = await _userManager.FindByNameAsync(userName)
            ?? throw new InvalidOperationException("User with provided user name doesn't exist.");

        return await CreateUserResponseAsync(user);
    }

    public async Task<UserResponse> FindCurrentUserByEmailAsync(string email)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(email)
            ?? throw new InvalidOperationException("User with provided email doesn't exist.");

        return await CreateUserResponseAsync(user);
    }

    public async Task<UserResponse> CreateUserResponseAsync(ApplicationUser user)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user);

        return new UserResponse()
        {
            UserName = user.UserName!,
            Email = user.Email!,
            Id = user.Id,
            AccountId = user.AccountId,
            LockoutEnd = user.LockoutEnd,
            Role = roles.First()
        };
    }

     public async Task<UserResponse> SignUpAsync(SignUpRequest payload)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(payload.UserName);

        if (user != null)
            throw new EndUserException("UserName already taken.");

        user = new()
        {
            UserName = payload.UserName,
            Email = payload.Email
        };

        IdentityResult result = await _userManager.CreateAsync(user, payload.Password);

        if (result.Succeeded == false)
            throw new InvalidOperationException(result.Errors.First().Description);

        result = await _userManager.AddToRoleAsync(user, ApplicationRoles.User);

        if (result.Succeeded == false)
            throw new InvalidOperationException(result.Errors.First().Description);

        _logger.LogInformation("Account {userName} with email {email} has been made.", payload.UserName, payload.Email);
 
        var appUser = await _accountRepository.CreateBlankProfileAsync(payload.UserName, payload.Email);
        
        user.AccountId = appUser.Id;
        await _userManager.UpdateAsync(user);

        return await CreateUserResponseAsync(user);
    }

    public async Task UpdateEmailAsync(string email, string newEmail)
    { 
        var newEmailUser = await _userManager.FindByEmailAsync(newEmail);
        if(newEmailUser is not null)
            throw new Exception("There is already a user with provided email!");
        
        var user = await _userManager.FindByEmailAsync(email);
        if(user is null)
            throw new Exception("No user found for provided email address!");

        var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
        await _userManager.ChangeEmailAsync(user, newEmail, token);
    }

    public async Task UpdateUserNameAsync(string userName, string newUserName)
    { 
        var newUserNameUser = await _userManager.FindByNameAsync(newUserName);
        if(newUserNameUser is not null)
            throw new Exception("There is already a user with provided userName!");
        
        var user = await _userManager.FindByNameAsync(userName);
        if(user is null)
            throw new Exception("No user found for provided userName!");

        await _userManager.SetUserNameAsync(user, newUserName);
    }
}
