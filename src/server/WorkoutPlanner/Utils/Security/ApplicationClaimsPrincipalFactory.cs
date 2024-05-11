using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using WorkoutPlanner.Models.Auth;

namespace WorkoutPlanner.Api.Utils.Security;

public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    // zmiana wartości wymusi wylogowanie użytkowników
    private readonly string _scopeVersion;

    public ApplicationClaimsPrincipalFactory(
        IConfiguration cfg,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
    {
        RoleManager = roleManager;
        _scopeVersion = cfg.GetValue<string>("Security:ScopeVersion") ?? "";
    }

    public RoleManager<ApplicationRole> RoleManager { get; private set; }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        ClaimsIdentity id = await base.GenerateClaimsAsync(user);
        if (UserManager.SupportsUserRole)
        {
            Dictionary<string, Claim> rolesClaims = new();

            IList<string> roles = await UserManager.GetRolesAsync(user);
            foreach (string? roleName in roles)
            {
                id.AddClaim(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));
                if (RoleManager.SupportsRoleClaims)
                {
                    ApplicationRole? role = await RoleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        IList<Claim> claims = await RoleManager.GetClaimsAsync(role);
                        foreach (Claim claim in claims)
                        {
                            if (!rolesClaims.ContainsKey(claim.Type)
                                || rolesClaims[claim.Type].Value == "False")
                            {
                                rolesClaims[claim.Type] = claim;
                            }
                        }
                    }
                }
            }

            foreach (Claim claim in rolesClaims.Values)
            {
                if (!id.Claims.Any(c => c.Type == claim.Type))
                {
                    id.AddClaim(claim);
                }
            }
        }

        id.AddClaim(new Claim("scope", _scopeVersion));

        return id;
    }
}
