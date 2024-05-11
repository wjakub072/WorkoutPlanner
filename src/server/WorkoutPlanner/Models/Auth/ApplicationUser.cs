using Microsoft.AspNetCore.Identity;

namespace WorkoutPlanner.Models.Auth;

public class ApplicationUser : IdentityUser<int>
{
    public int AccountId { get; set; }
}
