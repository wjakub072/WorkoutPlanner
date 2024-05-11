using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WorkoutPlanner.Models.Auth;

namespace WorkoutPlanner.Api.Utils.Security;

public class UserAuthorizationHandler : AuthorizationHandler<ResourceOperationRequirement, IdentityUser<int>>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, IdentityUser<int> resource)
    {
        if (context.User.IsInRole(ApplicationRoles.Admin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        };

        if (requirement.ResourceOperation == ResourceOperation.Read
            || requirement.ResourceOperation == ResourceOperation.Update)
        {
            var claim = context.User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);

            if (int.TryParse(claim?.Value, out int userId))
            {
                if (resource.Id == userId)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
        }

        return Task.CompletedTask;
    }
}
